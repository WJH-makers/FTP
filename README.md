<p align="center">
  <img src="https://capsule-render.vercel.app/api?type=waving&color=0:141e30,100:243b55&height=180&section=header&text=FTP%20Client-Server&fontSize=50&fontColor=ffffff&animation=fadeIn&fontAlignY=38&desc=File%20Transfer%20Protocol%20%E2%80%93%20C%20(WinSock)%20%2B%20C%23%20(.NET)%20Dual%20Stack&descAlignY=55&descAlign=50" width="100%" />
</p>

| Category | Stack |
|----------|-------|
| **Language** | C (WinSock2), C# (.NET 6.0) |
| **UI** | Windows Forms |
| **Protocol** | TCP/IP, custom packet framing |
| **Platform** | Windows only |

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

## ❓ FAQ

| Question | Answer |
|----------|--------|
| **Can I use this over the internet?** | Designed for LAN — no encryption or NAT traversal. Add TLS for WAN use. |
| **How does resume work?** | Client saves byte offset on interrupt; RETR with offset header tells server to skip already-transferred data. |
| **Which OS is supported?** | Windows only (WinSock2 dependency). Linux port would require POSIX socket rewrite. |

## 🔗 See Also

- [File Management Tool](/WJH-makers/FileManagementTool) — Higher-level file operations (compress, scan, git) over a web interface

## 🎓 Academic Context

This project was completed for the **Computer Networks** course at **Wuhan University**, demonstrating TCP socket programming, protocol design, and file transfer mechanics.

---

<p align="center">
  <img src="https://capsule-render.vercel.app/api?type=waving&color=0:243b55,100:141e30&height=100&section=footer" width="100%" />
</p>
