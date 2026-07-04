<p align="center">
  <img src="https://capsule-render.vercel.app/api?type=waving&color=0:141e30,100:243b55&height=180&section=header&text=FTP%20Client-Server&fontSize=50&fontColor=ffffff&animation=fadeIn&fontAlignY=38&desc=File%20Transfer%20Protocol%20%E2%80%93%20C%20(WinSock)%20%2B%20C%23%20(.NET)%20Dual%20Stack&descAlignY=55&descAlign=50" width="100%" />
</p>

<p align="center">
  <img src="https://img.shields.io/badge/C-WinSock2-00599C?style=flat-square&logo=c" />
  <img src="https://img.shields.io/badge/C%23-.NET%206.0-512BD4?style=flat-square&logo=dotnet" />
  <img src="https://img.shields.io/badge/WinForms-GUI-239120?style=flat-square" />
  <img src="https://img.shields.io/badge/TCP-File%20Transfer-00A1E9?style=flat-square" />
  <img src="https://img.shields.io/badge/Feature-Resume%20(断点续传)-green?style=flat-square" />
</p>

## 📋 Overview

A dual-stack **FTP (File Transfer Protocol)** implementation featuring both **C (WinSock)** and **C# (.NET)** implementations. The project includes a full GUI client (WinForms), console clients, and a TCP-based server — all with **resume (断点续传)** support for large file transfers.

## ✨ Key Features

- **Dual Implementation**: C (low-level WinSock) + C# (.NET high-level) stacks
- **Resume Support**: Download/upload with resume for interrupted transfers
- **GUI Client**: Windows Forms interface for easy file operations
- **Multi-Client**: WinForms GUI, C# console, C console clients
- **FTP Commands**: Connect, list directories, upload, download
- **TCP Protocol**: Custom packet-based protocol over TCP sockets

## 🏗️ Architecture

```
FTP/
├── FTP/                        # C# WinForms GUI Client
│   ├── Form1.cs                # Main UI (connect, browse, transfer)
│   ├── FTPclient.cs            # FTP client protocol logic
│   └── Program.cs              # Application entry point
├── FTPclient/                  # C Console Client (WinSock)
│   ├── client.c / client.h     # Socket operations & file transfer
│   └── Program.cs              # .NET wrapper entry
├── FTPserver/                  # C Console Server (WinSock)
│   ├── server.c / server.h     # TCP listen, handle clients, file I/O
│   └── Program.cs              # .NET server with TcpListener
├── test/                       # Test utilities
└── FTP.sln                     # Visual Studio solution
```

### Protocol Flow

```
Client                      Server
  │                           │
  ├── Connect (TCP) ───────> │
  │ <── Welcome ─────────────┤
  ├── LIST ─────────────────> │
  │ <── Directory Listing ───┤
  ├── RETR filename ────────> │
  │ <── File Data (chunks) ──┤
  ├── STOR filename ────────> │
  │ <── ACK ────────────────┤
  └── QUIT ─────────────────> │
```

## 🚀 Quick Start

### Prerequisites

- Windows OS (WinSock2)
- Visual Studio 2022+ / .NET 6.0 SDK
- C compiler (MSVC)

### Build & Run

```bash
# Open solution
open FTP.sln

# Build all projects
dotnet build

# Start server
cd FTPserver
dotnet run

# Start GUI client
cd FTP
dotnet run
```

### Usage

```bash
# Server (default port 21)
FTPserver.exe

# Client - GUI mode
FTP.exe

# Client - Console mode
FTPclient.exe <server> <username> <password>
```

## 🎓 Academic Context

This project was completed for the **Computer Networks** course at **Wuhan University**, demonstrating TCP socket programming, protocol design, and file transfer mechanics.

---

<p align="center">
  <img src="https://capsule-render.vercel.app/api?type=waving&color=0:243b55,100:141e30&height=100&section=footer" width="100%" />
</p>
