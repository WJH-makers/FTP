#include"server.h"

char recvBuf[1024];//接受客户端发送的消息
int fileSize;
char* fileBuf;
char* fileName;

int main() {
	initSocket();
	listenToClient();
	closeSocket();
	return 0;
}


bool initSocket() {

	WSADATA wsadata;
	if (WSAStartup(MAKEWORD(2, 2), &wsadata) != 0) {
		printf("WSAStartup failed: %d\n", WSAGetLastError());
		return false;
	}
	return true;
}

bool closeSocket() {
	if (WSACleanup() != 0) {
		printf("WSACleanUP failed: %d\n", WSAGetLastError());
		return false;
	}
	return true;
}

void listenToClient() {
	//IPV4 流式传输 TCP协议
	SOCKET serfd = socket(AF_INET, SOCK_STREAM, IPPROTO_TCP);
	if (serfd == INVALID_SOCKET) {
		printf("socket failed: %d\n", WSAGetLastError());
		return;
	}
	struct sockaddr_in serAddr;
	serAddr.sin_family = AF_INET;//与上述创建的socket一样
	serAddr.sin_port = htons(SPORT);//本地字节序转化为网络字节序
	serAddr.sin_addr.S_un.S_addr = ADDR_ANY;//监听所有端口
	if (bind(serfd, (struct sockaddr*)&serAddr, sizeof(serAddr)) != 0) {
		printf("bind failed: %d", WSAGetLastError());
		return;
	}
	//监听过程
	if (listen(serfd, 10) != 0) {
		printf("listen failed: %d", WSAGetLastError());
		return;
	}
	//接受客户端时候检验客户端的来源
	struct sockaddr_in cliAddr;
	int len = sizeof(cliAddr);
	//第二步 服务器端监听到信息
	SOCKET clifd = accept(serfd, (struct sockaddr*)&cliAddr, &len);
	if (INVALID_SOCKET == clifd) {
		printf("accept failed: %d", WSAGetLastError());
		return;
	}
	while (processMsg(clifd)) {
		//Sleep(5000);
	}
}





bool processMsg(SOCKET s) {
	//成功返回消息，失败返回零
	int  msg = recv(s, recvBuf, 1024, 0);
	if (msg <= 0) {
		printf("客户端下线... 报错码为: %d\n", WSAGetLastError());
		return false;
	}
	struct MsgHeader* Msg = (struct MsgHeader*)recvBuf;
	struct MsgHeader exitMsg = { .msgID = MSG_SUCCESS };
	struct MsgHeader recvMsg = { .msgID = MSG_RECV };
	switch (Msg->msgID) {
	case MSG_UPLOAD:
		printf("开始接收客户端发送的信息!\n");
		if (send(s, (char*)&recvMsg, sizeof(struct MsgHeader), 0) == SOCKET_ERROR) {
			printf("发送失败: %d\n", WSAGetLastError());
			return false;
		}






		break;
	case MSG_FILENAME:
		//第三步 读取客户端要求的信息
		readFile(s, Msg);
		break;
	case MSG_SEND:
		//第六步 开始发送文件
		printf("开始发送信息!\n");
		sendFile(s);
		break;
	case MSG_SUCCESS:
		//第九步 客户端传来接收成功的消息,断开连接
		if (send(s, (char*)&exitMsg, sizeof(struct MsgHeader), 0) == SOCKET_ERROR) {
			printf("发送失败 : %d\n", WSAGetLastError());
			return false;
		}
		printf("传输完成！");
		return true;
	}
	return true;
}


bool readFile(SOCKET clifd, struct MsgHeader* msg) {
	char* path = msg->fileInfo.fileName;
	printf("%s\n", path);
	FILE* pread = fopen(path, "rb");
	struct MsgHeader error = { .msgID = MSG_OPEN_FAIL };
	strcpy(error.fileInfo.fileName, path);
	if (pread == NULL) {
		printf("无法找到[%s]文件\n", path);
		if (SOCKET_ERROR == send(clifd, (char*)&error, sizeof(struct MsgHeader), 0)) {
			printf("send failed: %d\n", WSAGetLastError());
		}
		return false;
	}
	else {
		//获取文件大小
		fseek(pread, 0, SEEK_END);//从最后读取文件
		fileSize = ftell(pread);
		fseek(pread, 0, SEEK_SET);//读到文件最前面
		struct MsgHeader Msg = { .msgID = MSG_FILESIZE ,.fileInfo.fileSize = fileSize };
		char tfname[200] = { 0 }, text[100];
		_splitpath(msg->fileInfo.fileName, NULL, NULL, tfname, text);//分割地址
		strcat(tfname, text);
		strcpy(Msg.fileInfo.fileName, tfname);
		//第四步 返回客户端需要的信息的长度
		send(clifd, (char*)&Msg, sizeof(struct MsgHeader), 0);
		printf("已经完成读取文件!\n");
		fileBuf = calloc(fileSize + 1, sizeof(char));
		if (fileBuf == NULL) {
			printf("内存不足: %d", WSAGetLastError());
			return false;
		}
		fread(fileBuf, sizeof(char), fileSize, pread);
		fclose(pread);
		return true;
	}
}


bool sendFile(SOCKET s) {
	struct MsgHeader Msg = { .msgID = MSG_READY };
	for (int i = 0;i < fileSize; i += PACKET_SIZE) {
		Msg.packet.nstart = i;
		if (i + PACKET_SIZE + 1 > fileSize) {
			Msg.packet.nsize = fileSize - i;
		}
		else {
			Msg.packet.nsize = PACKET_SIZE;
		}
		printf("已经发送了: %d字节\n", Msg.packet.nsize);
		memcpy(Msg.packet.buf, fileBuf + Msg.packet.nstart, Msg.packet.nsize);
		//第七步 发送文件成功
		if (send(s, (char*)&Msg, sizeof(struct MsgHeader), 0) == SOCKET_ERROR) {
			printf("文件发送失败 : %d:\n", WSAGetLastError());
			return false;
		}

		//接收断点重传的信息
		char buf[1024] = { 0 };
		recv(s, buf, 1024, 0);
		struct MsgHeader* msg = (struct MsgHeader*)buf;
		if (msg->msgID == MSG_SLEEP) {
			Sleep(100);
		}
	}
	return true;
}