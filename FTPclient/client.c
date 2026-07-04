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
	recv(s, recvBuf, 1024, 0);
	struct MsgHeader* msg = (struct MsgHeader*)recvBuf;
	switch (msg->msgID)
	{
		//���յ����������ϴ����� �����ϴ����ļ���С
	case MSG_RECV:





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
	send(s, (char*)&file, sizeof(struct MsgHeader), 0);// +1����Ϊ'\0'(���ַ�)
}

//�ϴ��ļ� �ͻ��˸��߷���˽�Ҫ�ϴ��ļ�
void update(SOCKET s) {
	gets_s(sendFile, 1023);
	struct MsgHeader file = { .msgID = MSG_UPLOAD };
	strcpy(file.fileInfo.fileName, sendFile);
	send(s, (char*)&file, sizeof(struct MsgHeader), 0);
}




















void readyread(SOCKET s, struct MsgHeader* msg) {
	fileSize = msg->fileInfo.fileSize;
	strcpy(fileName, msg->fileInfo.fileName);
	fileBuf = calloc(fileSize + 1, sizeof(char));//�����ڴ�ռ�
	if (fileBuf == NULL) {
		printf("�ڴ治�㣬������\n");
		return;
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



	//�ϵ��ش�
	struct MsgHeader sleep = { .msgID = MSG_SLEEP };
	printf("��ȴ�0.1��!\n");
	send(s, (char*)&sleep, sizeof(struct MsgHeader), 0);


	//�ڰ˲� �����ļ��ɹ�
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
		printf("�������\n");
		return true;
	}
	return true;
}