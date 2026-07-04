<p align="center">
  <img src="https://capsule-render.vercel.app/api?type=waving&color=0:141e30,100:243b55&height=180&section=header&text=FTP%20Client-Server&fontSize=50&fontColor=ffffff&animation=fadeIn&fontAlignY=38&desc=File%20Transfer%20Protocol%20%E2%80%93%20C%20(WinSock)%20%2B%20C%23%20(.NET)%20Dual%20Stack&descAlignY=55&descAlign=50" width="100%" />
</p>

<p align="center">
  <img src="https://img.shields.io/badge/C-WinSock2-00599C?style=flat-square&logo=c" />
  <img src="https://img.shields.io/badge/C%23-.NET%206.0-512BD4?style=flat-square&logo=dotnet" />
  <img src="https://img.shields.io/badge/WinForms-GUI-239120?style=flat-square" />
  <img src="https://img.shields.io/badge/TCP-File%20Transfer-00A1E9?style=flat-square" />
  <img src="https://img.shields.io/badge/Feature-Resume%20(%E6%96%AD%E7%82%B9%E7%BB%AD%E4%BC%A0)-green?style=flat-square" />
</p>

## 📋 Overview

A dual-stack **FTP** implementation in **C (WinSock)** and **C# (.NET)**, featuring a WinForms GUI client, console clients, and TCP server — all with **resume (断点续传)** support for interrupted large file transfers.

> **Why dual-stack?** The C stack demonstrates low-level socket mechanics (raw WinSock2, manual packet framing); the C# stack shows rapid application development (async TcpListener, WinForms). Together they trace the full spectrum from protocol implementation to user-facing product.

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

## 🎓 Academic Context

This project was completed for the **Computer Networks** course at **Wuhan University**, demonstrating TCP socket programming, protocol design, and file transfer mechanics.

---

<p align="center">
  <img src="https://capsule-render.vercel.app/api?type=waving&color=0:243b55,100:141e30&height=100&section=footer" width="100%" />
</p>
