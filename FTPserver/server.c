#include"server.h"

char recvBuf[1024];//���ܿͻ��˷��͵���Ϣ
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
	//IPV4 ��ʽ���� TCPЭ��
	SOCKET serfd = socket(AF_INET, SOCK_STREAM, IPPROTO_TCP);
	if (serfd == INVALID_SOCKET) {
		printf("socket failed: %d\n", WSAGetLastError());
		return;
	}
	struct sockaddr_in serAddr;
	serAddr.sin_family = AF_INET;//������������socketһ��
	serAddr.sin_port = htons(SPORT);//�����ֽ���ת��Ϊ�����ֽ���
	serAddr.sin_addr.S_un.S_addr = ADDR_ANY;//�������ж˿�
	if (bind(serfd, (struct sockaddr*)&serAddr, sizeof(serAddr)) != 0) {
		printf("bind failed: %d", WSAGetLastError());
		closesocket(serfd);
		return;
	}
	//��������
	if (listen(serfd, 10) != 0) {
		printf("listen failed: %d", WSAGetLastError());
		closesocket(serfd);
		return;
	}
	//���ܿͻ���ʱ�����ͻ��˵���Դ
	struct sockaddr_in cliAddr;
	int len = sizeof(cliAddr);
	//�ڶ��� �������˼�������Ϣ
	SOCKET clifd = accept(serfd, (struct sockaddr*)&cliAddr, &len);
	if (INVALID_SOCKET == clifd) {
		printf("accept failed: %d", WSAGetLastError());
		closesocket(serfd);
		return;
	}
	while (processMsg(clifd)) {
		//Sleep(5000);
	}
}





bool processMsg(SOCKET s) {
	//�ɹ�������Ϣ��ʧ�ܷ�����
	int  msg = recv(s, recvBuf, 1024, 0);
	if (msg <= 0) {
		printf("�ͻ�������... ������Ϊ: %d\n", WSAGetLastError());
		return false;
	}
	struct MsgHeader* Msg = (struct MsgHeader*)recvBuf;
	struct MsgHeader exitMsg = { .msgID = MSG_SUCCESS };
	struct MsgHeader recvMsg = { .msgID = MSG_RECV };
	switch (Msg->msgID) {
	case MSG_UPLOAD:
		printf("��ʼ���տͻ��˷��͵���Ϣ!\n");
		if (send(s, (char*)&recvMsg, sizeof(struct MsgHeader), 0) == SOCKET_ERROR) {
			printf("����ʧ��: %d\n", WSAGetLastError());
			return false;
		}






		break;
	case MSG_FILENAME:
		//������ ��ȡ�ͻ���Ҫ�����Ϣ
		readFile(s, Msg);
		break;
	case MSG_SEND:
		//������ ��ʼ�����ļ�
		printf("��ʼ������Ϣ!\n");
		sendFile(s);
		break;
	case MSG_SUCCESS:
		//�ھŲ� �ͻ��˴������ճɹ�����Ϣ,�Ͽ�����
		if (send(s, (char*)&exitMsg, sizeof(struct MsgHeader), 0) == SOCKET_ERROR) {
			printf("����ʧ�� : %d\n", WSAGetLastError());
			return false;
		}
		printf("������ɣ�");
		return true;
	}
	return true;
}


bool readFile(SOCKET clifd, struct MsgHeader* msg) {
	char* path = msg->fileInfo.fileName;
	printf("%s\n", path);
	char fullPath[_MAX_PATH];
	if (_fullpath(fullPath, path, _MAX_PATH) == NULL) {
		printf("Invalid path\n");
		struct MsgHeader error = { .msgID = MSG_OPEN_FAIL };
		strcpy(error.fileInfo.fileName, path);
		send(clifd, (char*)&error, sizeof(struct MsgHeader), 0);
		return false;
	}
	if (strncmp(fullPath, ".\\", 2) != 0 && strncmp(fullPath, ".", 1) != 0) {
		printf("Path traversal detected: %s\n", fullPath);
		struct MsgHeader error = { .msgID = MSG_OPEN_FAIL };
		strcpy(error.fileInfo.fileName, path);
		send(clifd, (char*)&error, sizeof(struct MsgHeader), 0);
		return false;
	}
	FILE* pread = fopen(path, "rb");
	struct MsgHeader error = { .msgID = MSG_OPEN_FAIL };
	strcpy(error.fileInfo.fileName, path);
	if (pread == NULL) {
		printf("�޷��ҵ�[%s]�ļ�\n", path);
		if (SOCKET_ERROR == send(clifd, (char*)&error, sizeof(struct MsgHeader), 0)) {
			printf("send failed: %d\n", WSAGetLastError());
		}
		return false;
	}
	else {
		//��ȡ�ļ���С
		fseek(pread, 0, SEEK_END);//������ȡ�ļ�
		fileSize = ftell(pread);
		fseek(pread, 0, SEEK_SET);//�����ļ���ǰ��
		struct MsgHeader Msg = { .msgID = MSG_FILESIZE ,.fileInfo.fileSize = fileSize };
		char tfname[MAX_PATH] = {0}, text[MAX_PATH] = {0};
		_splitpath_s(msg->fileInfo.fileName, NULL, 0, NULL, 0, tfname, sizeof(tfname), text, sizeof(text));
		strcat_s(tfname, sizeof(tfname), text);
		strcpy(Msg.fileInfo.fileName, tfname);
		//���Ĳ� ���ؿͻ�����Ҫ����Ϣ�ĳ���
		send(clifd, (char*)&Msg, sizeof(struct MsgHeader), 0);
		printf("�Ѿ���ɶ�ȡ�ļ�!\n");
		fileBuf = calloc(fileSize + 1, sizeof(char));
		if (fileBuf == NULL) {
			printf("�ڴ治��: %d", WSAGetLastError());
			return false;
		}
		fread(fileBuf, sizeof(char), fileSize, pread);
		fclose(pread);
		return true;
	}
}


bool sendFile(SOCKET s) {
	struct MsgHeader Msg;
	memset(&Msg, 0, sizeof(Msg));
	Msg.msgID = MSG_READY;
	for (int i = 0;i < fileSize; i += PACKET_SIZE) {
		Msg.packet.nstart = i;
		if (i + PACKET_SIZE + 1 > fileSize) {
			Msg.packet.nsize = fileSize - i;
		}
		else {
			Msg.packet.nsize = PACKET_SIZE;
		}
		printf("�Ѿ�������: %d�ֽ�\n", Msg.packet.nsize);
		memcpy(Msg.packet.buf, fileBuf + Msg.packet.nstart, Msg.packet.nsize);
		//���߲� �����ļ��ɹ�
		if (send(s, (char*)&Msg, sizeof(struct MsgHeader), 0) == SOCKET_ERROR) {
			printf("�ļ�����ʧ�� : %d:\n", WSAGetLastError());
			free(fileBuf);
			fileBuf = NULL;
			return false;
		}

		//���նϵ��ش�����Ϣ
		char buf[1024] = { 0 };
		recv(s, buf, 1024, 0);
		struct MsgHeader* msg = (struct MsgHeader*)buf;
		if (msg->msgID == MSG_SLEEP) {
			Sleep(100);
		}
	}
	free(fileBuf);
	fileBuf = NULL;
	return true;
}