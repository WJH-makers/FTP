#include"server.h"

char recvBuf[1024];//���ܿͻ��˷��͵���Ϣ
int64_t fileSize;
char* fileBuf;
char* fileName;

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
	closesocket(clifd);
}





bool processMsg(SOCKET s) {
	//�ɹ�������Ϣ��ʧ�ܷ�����
	int  msg = recv_all(s, recvBuf, 1024);
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
		if (send_all(s, (char*)&recvMsg, sizeof(struct MsgHeader)) == -1) {
			printf("����ʧ��: %d\n", WSAGetLastError());
			return false;
		}
		recvFile(s, Msg);
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
		if (send_all(s, (char*)&exitMsg, sizeof(struct MsgHeader)) == -1) {
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
		send_all(clifd, (char*)&error, sizeof(struct MsgHeader));
		return false;
	}
	char cwd[_MAX_PATH];
	_getcwd(cwd, _MAX_PATH);
	if (_strnicmp(fullPath, cwd, strlen(cwd)) != 0) {
		printf("Path traversal detected: %s\n", fullPath);
		struct MsgHeader error = { .msgID = MSG_OPEN_FAIL };
		strcpy(error.fileInfo.fileName, path);
		send_all(clifd, (char*)&error, sizeof(struct MsgHeader));
		return false;
	}
	FILE* pread = fopen(path, "rb");
	struct MsgHeader error = { .msgID = MSG_OPEN_FAIL };
	strcpy(error.fileInfo.fileName, path);
	if (pread == NULL) {
		printf("�޷��ҵ�[%s]�ļ�\n", path);
		if (send_all(clifd, (char*)&error, sizeof(struct MsgHeader)) == -1) {
			printf("send failed: %d\n", WSAGetLastError());
		}
		return false;
	}
	else {
		//��ȡ�ļ���С
		_fseeki64(pread, 0, SEEK_END);//������ȡ�ļ�
		fileSize = _ftelli64(pread);
		_fseeki64(pread, 0, SEEK_SET);//�����ļ���ǰ��
		struct MsgHeader Msg = { .msgID = MSG_FILESIZE ,.fileInfo.fileSize = (int)fileSize };
		char tfname[MAX_PATH] = {0}, text[MAX_PATH] = {0};
		_splitpath_s(msg->fileInfo.fileName, NULL, 0, NULL, 0, tfname, sizeof(tfname), text, sizeof(text));
		strcat_s(tfname, sizeof(tfname), text);
		strcpy(Msg.fileInfo.fileName, tfname);
		//���Ĳ� ���ؿͻ�����Ҫ����Ϣ�ĳ���
		send_all(clifd, (char*)&Msg, sizeof(struct MsgHeader));
		printf("�Ѿ���ɶ�ȡ�ļ�!\n");
		if (fileSize > SIZE_MAX / sizeof(char) - 1) {
			printf("�ļ�����\n");
			fclose(pread);
			return false;
		}
		fileBuf = calloc((size_t)(fileSize + 1), sizeof(char));
		if (fileBuf == NULL) {
			printf("�ڴ治��: %d", WSAGetLastError());
			fclose(pread);
			return false;
		}
		fread(fileBuf, sizeof(char), (size_t)fileSize, pread);
		fclose(pread);
		return true;
	}
}


bool sendFile(SOCKET s) {
	struct MsgHeader Msg;
	memset(&Msg, 0, sizeof(Msg));
	Msg.msgID = MSG_READY;
	for (int64_t i = 0;i < fileSize; i += PACKET_SIZE) {
		Msg.packet.nstart = (int)i;
		if (i + PACKET_SIZE + 1 > fileSize) {
			Msg.packet.nsize = (int)(fileSize - i);
		}
		else {
			Msg.packet.nsize = PACKET_SIZE;
		}
		printf("�Ѿ�������: %d�ֽ�\n", Msg.packet.nsize);
		memcpy(Msg.packet.buf, fileBuf + Msg.packet.nstart, Msg.packet.nsize);
		//���߲� �����ļ��ɹ�
		if (send_all(s, (char*)&Msg, sizeof(struct MsgHeader)) == -1) {
			printf("�ļ�����ʧ�� : %d:\n", WSAGetLastError());
			free(fileBuf);
			fileBuf = NULL;
			return false;
		}

		//���նϵ��ش�����Ϣ
		char buf[1024] = { 0 };
		recv_all(s, buf, 1024);
		struct MsgHeader* msg = (struct MsgHeader*)buf;
		if (msg->msgID == MSG_SLEEP) {
			Sleep(100);
		}
	}
	free(fileBuf);
	fileBuf = NULL;
	return true;
}

bool recvFile(SOCKET s, struct MsgHeader* msg) {
	char* path = msg->fileInfo.fileName;
	char fullPath[_MAX_PATH];
	if (_fullpath(fullPath, path, _MAX_PATH) == NULL) {
		printf("Invalid path\n");
		return false;
	}
	char cwd[_MAX_PATH];
	_getcwd(cwd, _MAX_PATH);
	if (_strnicmp(fullPath, cwd, strlen(cwd)) != 0) {
		printf("Path traversal detected: %s\n", fullPath);
		return false;
	}
	FILE* f = fopen(path, "wb");
	if (f == NULL) {
		printf("无法打开文件: %s\n", path);
		return false;
	}
	// 接收文件数据包直到收到 MSG_SUCCESS
	char buf[sizeof(struct MsgHeader)];
	while (1) {
		if (recv_all(s, buf, sizeof(struct MsgHeader)) == -1) {
			printf("接收文件数据失败\n");
			fclose(f);
			return false;
		}
		struct MsgHeader* pkt = (struct MsgHeader*)buf;
		if (pkt->msgID == MSG_SUCCESS) {
			printf("文件上传完成: %s\n", path);
			fclose(f);
			return true;
		}
		if (pkt->msgID == MSG_READY) {
			fwrite(pkt->packet.buf, 1, pkt->packet.nsize, f);
			struct MsgHeader sleep = { .msgID = MSG_SLEEP };
			send_all(s, (char*)&sleep, sizeof(struct MsgHeader));
		}
	}
}