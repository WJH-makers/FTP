#include"client.h"
char recvBuf[1024];
char* fileBuf;
int fileSize;
char fileName[1024];
char sendFile[1024];
int main() {
	initSocket();
	connectTohost();
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

void connectTohost() {
	//IPV4 流式传输 TCP协议
	SOCKET serfd = socket(AF_INET, SOCK_STREAM, IPPROTO_TCP);
	if (serfd == INVALID_SOCKET) {
		printf("socket failed: %d\n", WSAGetLastError());
		return;
	}
	struct sockaddr_in serAddr;
	serAddr.sin_family = AF_INET;//与上述创建的socket一样
	serAddr.sin_port = htons(SPORT);//本地字节序转化为网络字节序
	serAddr.sin_addr.S_un.S_addr = inet_addr("127.0.0.1");//服务器IP地址

	//连接到服务器
	if (connect(serfd, (struct sockaddr*)&serAddr, sizeof(serAddr)) != 0) {
		printf("connect failed: %d\n", WSAGetLastError());
		return;
	}
	//download(serfd);
	update(serfd);
	while (processMsg(serfd)) {
		//Sleep(5000);
	}
}


bool processMsg(SOCKET s) {
	recv(s, recvBuf, 1024, 0);
	struct MsgHeader* msg = (struct MsgHeader*)recvBuf;
	switch (msg->msgID)
	{
		//接收到服务器的上传允许 发送上传的文件大小
	case MSG_RECV:





		break;
	case MSG_OPEN_FAIL:
		download(s);
		break;
	case MSG_FILESIZE:
		//第五步 获取接受到的文件大小和文件名
		readyread(s, msg);
		break;
	case MSG_READY:
		writeFile(s, msg, 0);
		break;
	case MSG_SUCCESS:
		printf("再见!\n");
		return false;
		break;
	}
	return true;
}




//第一步 客户端发送要下载文件信息
void download(SOCKET s) {
	gets_s(fileName, 1023);//输入文件名
	struct MsgHeader file = { .msgID = MSG_FILENAME };
	strcpy(file.fileInfo.fileName, fileName);//使得获取文件名没有问题
	send(s, (char*)&file, sizeof(struct MsgHeader), 0);// +1是因为'\0'(空字符)
}

//上传文件 客户端告诉服务端将要上传文件
void update(SOCKET s) {
	gets_s(sendFile, 1023);
	struct MsgHeader file = { .msgID = MSG_UPLOAD };
	strcpy(file.fileInfo.fileName, sendFile);
	send(s, (char*)&file, sizeof(struct MsgHeader), 0);
}




















void readyread(SOCKET s, struct MsgHeader* msg) {
	fileSize = msg->fileInfo.fileSize;
	strcpy(fileName, msg->fileInfo.fileName);
	fileBuf = calloc(fileSize + 1, sizeof(char));//申请内存空间
	if (fileBuf == NULL) {
		printf("内存不足，请重试\n");
	}
	else {
		struct MsgHeader Msg = { .msgID = MSG_SEND };
		if (send(s, (char*)&Msg, sizeof(struct MsgHeader), 0) == SOCKET_ERROR) {
			printf("send error: %d", WSAGetLastError());
			return;
		}
		else {
			printf("size: %d filename: %s\n", msg->fileInfo.fileSize, msg->fileInfo.fileName);
		}
	}
}



bool writeFile(SOCKET s, struct MsgHeader* Msg) {
	if (fileBuf == NULL)return false;
	int nsize = Msg->packet.nsize;
	int nstart = Msg->packet.nstart;
	memcpy(fileBuf + Msg->packet.nstart, Msg->packet.buf, Msg->packet.nsize);
	printf("packet: %d  %d\n", nstart + nsize, fileSize);



	//断点重传
	struct MsgHeader sleep = { .msgID = MSG_SLEEP };
	printf("请等待0.1秒!\n");
	send(s, (char*)&sleep, sizeof(struct MsgHeader), 0);


	//第八步 接受文件成功
	if (nstart + nsize >= fileSize) {
		FILE* pwrite = fopen(fileName, "wb");
		if (pwrite == NULL) {
			printf("write file error ..\n");
			return false;
		}
		fwrite(fileBuf, sizeof(char), fileSize, pwrite);
		fclose(pwrite);
		free(fileBuf);
		fileBuf = NULL;
		struct MsgHeader msg = { .msgID = MSG_SUCCESS };
		send(s, (char*)&msg, sizeof(struct MsgHeader), 0);
		printf("传输完成\n");
		return true;
	}
	return true;
}