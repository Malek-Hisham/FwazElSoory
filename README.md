# FwazElSoory — Restaurant Order Management System 🍽️

A full-featured **desktop application** for managing restaurant orders across kitchen and front-of-house staff — built with C# and .NET 8 Windows Forms.

## Features
- 📋 **New Order** — Create and customize customer orders
- 👨‍🍳 **Kitchen View** — Real-time order display for kitchen staff
- 📊 **Orders List** — Full overview of all active and past orders
- 🔍 **Order Details** — Drill into any order's full breakdown
- 🎨 **Custom UI Theme** — Clean, consistent visual design across all forms

## Screenshots
> Coming soon

## Tech Stack
![C#](https://img.shields.io/badge/C%23-239120?style=flat-square&logo=c-sharp&logoColor=white)
![.NET 8](https://img.shields.io/badge/.NET_8-512BD4?style=flat-square&logo=dotnet&logoColor=white)
![WinForms](https://img.shields.io/badge/WinForms-0078D4?style=flat-square&logo=windows&logoColor=white)

## Project Structure
```
FwazElSoory/
├── Forms/
│   ├── MainForm.cs         # Main navigation hub
│   ├── NewOrderForm.cs     # Create new orders
│   ├── KitchenForm.cs      # Kitchen display
│   ├── OrdersListForm.cs   # All orders overview
│   ├── OrderDetailForm.cs  # Single order details
│   └── AppTheme.cs         # Shared UI theme
├── Models/
│   ├── MenuItem.cs
│   ├── Order.cs
│   └── OrderItem.cs
└── Data/
    └── DataStore.cs        # In-memory data layer
```

## How to Run
1. Clone the repo
2. Open `FwazElSoory.csproj` in Visual Studio 2022
3. Build and run (F5)

## Requirements
- Visual Studio 2022
- .NET 8 SDK
- Windows OS

## Author
**Malek Hisham Moselhy** — [LinkedIn](https://www.linkedin.com/in/malek-hisham-8005882a0) · [GitHub](https://github.com/Malek-Hisham)
