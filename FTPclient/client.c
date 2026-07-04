#include"client.h"
char recvBuf[1024];
char* fileBuf;
int64_t fileSize;
char fileName[1024];
char sendFile[1024];

int recv_all(SOCKET s, char* buf, int len) {
	int total = 0;
	while (total < len) {
		int n = recv(s, buf + total, len - total, 0);
		if (n == SOCKET_ERROR || n == 0) return -1;
		total += n;
	}
	return total;
}

int send_all(SOCKET s, const char* buf, int len) {
	int total = 0;
	while (total < len) {
		int n = send(s, buf + total, len - total, 0);
		if (n == SOCKET_ERROR) return -1;
		total += n;
	}
	return total;
}

int main() {
	if (!initSocket()) return 1;
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
	//IPV4 ��ʽ���� TCPЭ��
	SOCKET serfd = socket(AF_INET, SOCK_STREAM, IPPROTO_TCP);
	if (serfd == INVALID_SOCKET) {
		printf("socket failed: %d\n", WSAGetLastError());
		return;
	}
	struct sockaddr_in serAddr;
	serAddr.sin_family = AF_INET;//������������socketһ��
	serAddr.sin_port = htons(SPORT);//�����ֽ���ת��Ϊ�����ֽ���
	serAddr.sin_addr.S_un.S_addr = inet_addr("127.0.0.1");//������IP��ַ

	//���ӵ�������
	if (connect(serfd, (struct sockaddr*)&serAddr, sizeof(serAddr)) != 0) {
		printf("connect failed: %d\n", WSAGetLastError());
		closesocket(serfd);
		return;
	}
	//download(serfd);
	update(serfd);
	while (processMsg(serfd)) {
		//Sleep(5000);
	}
}


bool processMsg(SOCKET s) {
	if (recv_all(s, recvBuf, 1024) == -1) {
		printf("接收失败\n");
		return false;
	}
	struct MsgHeader* msg = (struct MsgHeader*)recvBuf;
	switch (msg->msgID)
	{
		//���յ����������ϴ����� �����ϴ����ļ���С
	case MSG_RECV:
		sendFileFromDisk(s, sendFile);
		break;
	case MSG_OPEN_FAIL:
		download(s);
		break;
	case MSG_FILESIZE:
		//���岽 ��ȡ���ܵ����ļ���С���ļ���
		readyread(s, msg);
		break;
	case MSG_READY:
		writeFile(s, msg, 0);
		break;
	case MSG_SUCCESS:
		printf("�ټ�!\n");
		return false;
		break;
	}
	return true;
}




//��һ�� �ͻ��˷���Ҫ�����ļ���Ϣ
void download(SOCKET s) {
	gets_s(fileName, 1023);//�����ļ���
	struct MsgHeader file = { .msgID = MSG_FILENAME };
	strcpy(file.fileInfo.fileName, fileName);//ʹ�û�ȡ�ļ���û������
	send_all(s, (char*)&file, sizeof(struct MsgHeader));// +1����Ϊ'\0'(���ַ�)
}

//�ϴ��ļ� �ͻ��˸��߷���˽�Ҫ�ϴ��ļ�
void update(SOCKET s) {
	gets_s(sendFile, 1023);
	struct MsgHeader file = { .msgID = MSG_UPLOAD };
	strcpy(file.fileInfo.fileName, sendFile);
	send_all(s, (char*)&file, sizeof(struct MsgHeader));
	// 接收 MSG_RECV �Ͽɺ��ļ����ݻ��� processMsg �е� MSG_RECV ������ sendFileFromDisk ����
}




















void readyread(SOCKET s, struct MsgHeader* msg) {
	fileSize = msg->fileInfo.fileSize;
	strcpy(fileName, msg->fileInfo.fileName);
	if (fileSize > SIZE_MAX / sizeof(char) - 1) {
		printf("�ļ�����\n");
		return;
	}
	fileBuf = calloc((size_t)(fileSize + 1), sizeof(char));//�����ڴ�ռ�
	if (fileBuf == NULL) {
		printf("�ڴ治�㣬������\n");
		return;
	}
	else {
		struct MsgHeader Msg = { .msgID = MSG_SEND };
		if (send_all(s, (char*)&Msg, sizeof(struct MsgHeader)) == -1) {
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
	printf("packet: %d  %d\n", nstart + nsize, (int)fileSize);



	//�ϵ��ش�
	struct MsgHeader sleep;
	memset(&sleep, 0, sizeof(sleep));
	sleep.msgID = MSG_SLEEP;
	printf("��ȴ�0.1��!\n");
	send_all(s, (char*)&sleep, sizeof(struct MsgHeader));


	//�ڰ˲� �����ļ��ɹ�
	if (nstart + nsize >= fileSize) {
		FILE* pwrite = fopen(fileName, "wb");
		if (pwrite == NULL) {
			printf("write file error ..\n");
			return false;
		}
		fwrite(fileBuf, sizeof(char), (size_t)fileSize, pwrite);
		fclose(pwrite);
		free(fileBuf);
		fileBuf = NULL;
		struct MsgHeader msg;
		memset(&msg, 0, sizeof(msg));
		msg.msgID = MSG_SUCCESS;
		send_all(s, (char*)&msg, sizeof(struct MsgHeader));
		printf("�������\n");
		return true;
	}
	return true;
}

void sendFileFromDisk(SOCKET s, char* path) {
	FILE* f = fopen(path, "rb");
	if (f == NULL) {
		printf("无法打开文件: %s\n", path);
		return;
	}

	_fseeki64(f, 0, SEEK_END);
	int64_t fsize = _ftelli64(f);
	_fseeki64(f, 0, SEEK_SET);

	char* fbuf = (char*)calloc((size_t)(fsize + 1), sizeof(char));
	if (fbuf == NULL) {
		printf("内存不足\n");
		fclose(f);
		return;
	}
	fread(fbuf, sizeof(char), (size_t)fsize, f);
	fclose(f);

	struct MsgHeader pkt;
	memset(&pkt, 0, sizeof(pkt));

	for (int64_t i = 0; i < fsize; i += PACKET_SIZE) {
		pkt.msgID = MSG_READY;
		pkt.packet.nstart = (int)i;
		if (i + PACKET_SIZE + 1 > fsize) {
			pkt.packet.nsize = (int)(fsize - i);
		} else {
			pkt.packet.nsize = PACKET_SIZE;
		}
		memcpy(pkt.packet.buf, fbuf + i, pkt.packet.nsize);
		send_all(s, (char*)&pkt, sizeof(struct MsgHeader));

		char ack[1024] = {0};
		recv_all(s, ack, 1024);
	}

	free(fbuf);
	memset(&pkt, 0, sizeof(pkt));
	pkt.msgID = MSG_SUCCESS;
	send_all(s, (char*)&pkt, sizeof(struct MsgHeader));
}