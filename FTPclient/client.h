#pragma once
#include<WinSock2.h>
#pragma comment(lib,"ws2_32.lib")
#include<stdbool.h>
#include<stdio.h>
#include "dirent.h"
#include <stdint.h>
#define SPORT 8888 //�������˿ں�




enum MSGTAG {
	MSG_FILENAME = 1,
	MSG_FILESIZE = 2,
	MSG_READY = 3,
	MSG_SEND = 4,
	MSG_SUCCESS = 5,
	MSG_OPEN_FAIL = 6,
	MSG_SLEEP = 7,
	MSG_WAKE = 8,
	MSG_UPLOAD = 9,
	MSG_RECV = 10,
};



#pragma pack(1)//���ö��뷽ʽ
#define PACKET_SIZE (1024-sizeof(int)*3)
struct MsgHeader {
	enum MSGTAG msgID;
	union MyUnion {
		struct {
			int fileSize;
			char fileName[256];
		}fileInfo;
		struct {
			int nsize;//���Ĵ�С
			int nstart;//���ı��
			char buf[PACKET_SIZE];
		}packet;
	};

};
#pragma pack()



bool initSocket();

bool closeSocket();

void connectTohost();

bool processMsg(SOCKET);

void download(SOCKET);

void readyread(SOCKET, struct MsgHeader*);

bool writeFile(SOCKET, struct MsgHeader*);

void update(SOCKET);
void sendFileFromDisk(SOCKET, char*);

