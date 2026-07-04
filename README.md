<p align="center">
  <img src="https://capsule-render.vercel.app/api?type=waving&color=0:141e30,100:243b55&height=180&section=header&text=FTP%20%E5%AE%A2%E6%88%B7%E7%AB%AF-%E6%9C%8D%E5%8A%A1%E7%AB%AF&fontSize=50&fontColor=ffffff&animation=fadeIn&fontAlignY=38&desc=C%20(WinSock)%20%2B%20C%23%20(.NET)%20%E5%8F%8C%E6%A0%88%E5%AE%9E%E7%8E%B0&descAlignY=55&descAlign=50" width="100%" />
</p>

| 类别 | 技术栈 |
|------|--------|
| **语言** | C (WinSock2), C# (.NET 6.0) |
| **界面** | Windows Forms |
| **协议** | TCP/IP，自定义包帧 |
| **平台** | Windows |

## 📋 简介

C (WinSock) 和 C# (.NET) 双栈实现 FTP 文件传输，含 WinForms 图形客户端、命令行客户端和 TCP 服务器，支持**断点续传**功能。

## 🚀 快速开始

```bash
# 打开解决方案
open FTP.sln

# 构建所有项目
dotnet build

# 启动服务器
cd FTPserver && dotnet run

# 启动图形客户端
cd FTP && dotnet run

# 命令行客户端
FTPclient.exe <服务器> <用户名> <密码>
```

## ✨ 功能特性

- **双栈实现**：C 底层 WinSock + C# .NET 高层封装
- **断点续传**：中断传输后可从断点继续
- **图形界面**：Windows Forms 文件操作界面
- **多客户端**：WinForms 图形、C# 命令行、C 控制台三种模式
- **FTP 命令**：连接、列目录、上传、下载

## 🏗️ 项目结构

```
FTP/
├── FTP/                    # C# WinForms 图形客户端
│   ├── Form1.cs            # 主界面（连接、浏览、传输）
│   ├── FTPclient.cs        # FTP 协议逻辑
│   └── Program.cs
├── FTPclient/              # C 命令行客户端（WinSock）
│   ├── client.c            # 套接字操作与文件传输
│   └── Program.cs          # .NET 包装入口
├── FTPserver/              # C 命令行服务器（WinSock）
│   ├── server.c            # TCP 监听、客户端处理、文件 I/O
│   └── Program.cs          # .NET TcpListener 服务器
├── test/                   # 测试工具
└── FTP.sln
```

## ❓ 常见问题

| 问题 | 回答 |
|------|------|
| **可以在互联网上使用吗？** | 无加密/NAT 穿透，仅限局域网；添加 TLS 可用于广域网 |
| **断点续传原理？** | 客户端记录中断时的字节偏移，重连时发送偏移头跳过已传数据 |
| **支持哪些系统？** | 仅 Windows（依赖 WinSock2），Linux 需移植 POSIX socket |

## 🔗 相关项目

- [File Management](/WJH-makers/FileManagementTool) — 更上层的 Web 文件操作（压缩、扫描、Git）

## 🎓 课程背景

武汉大学计算机学院 · 计算机网络课程设计。

---

<p align="center">
  <img src="https://capsule-render.vercel.app/api?type=waving&color=0:243b55,100:141e30&height=100&section=footer" width="100%" />
</p>
