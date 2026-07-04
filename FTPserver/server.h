#pragma once
#include<WinSock2.h>
#pragma comment(lib,"ws2_32.lib")
#include<stdbool.h>
#include <sys/stat.h>
#include<stdio.h>
#include "dirent.h"
#include <string.h>

#define SPORT 8888 //服务器端口号




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



#pragma pack(1)//设置对齐方式
#define PACKET_SIZE (1024-sizeof(int)*3)
struct MsgHeader {
	enum MSGTAG msgID;
	union MyUnion {
		struct {
			int fileSize;
			char fileName[256];
		}fileInfo;
		struct {
			int nsize;//包的大小
			int nstart;//包的编号
			char buf[PACKET_SIZE];
		}packet;
	};

};
#pragma pack()



bool initSocket();

bool closeSocket();

void listenToClient();

bool processMsg(SOCKET);

bool readFile(SOCKET, struct MsgHeader*);

bool sendFile(SOCKET);

