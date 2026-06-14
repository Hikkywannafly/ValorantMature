# Hướng Dẫn

[Về README](../../README.md) · [Discord hỗ trợ](https://discord.gg/DXX4x5TQRq)

## Mục Đích

Bản public dùng để minh bạch phần xử lý cục bộ: tìm thư mục `Paks`, sao lưu file logo VNG, xoá logo khi game chạy và khôi phục file gốc khi game đóng.

Bản này không bao gồm tính năng premium, backend, kiểm tra key, cloud sync, máu đỏ, xác chết hoặc đổi màu tâm qua cloud.

## Yêu Cầu

- Windows 10/11
- .NET 9 SDK nếu tự build source
- Đã cài VALORANT
- Có quyền truy cập thư mục `Paks` của game

## Build

```powershell
dotnet restore .\src-wpf\ValorantUnlocker.Wpf.csproj
dotnet build .\src-wpf\ValorantUnlocker.Wpf.csproj
```

Build bản tự chạy local:

```powershell
dotnet publish .\src-wpf\ValorantUnlocker.Wpf.csproj -c Release -o .\publish
```

## Cách Dùng

1. Mở app trước khi vào VALORANT.
2. Kiểm tra thư mục game đang được app tự nhận ở trang chính.
3. Nếu app không tự tìm được, bấm `Thư mục game` và chọn thư mục cài VALORANT hoặc thư mục `Paks`.
4. Giữ trạng thái theo dõi đang bật.
5. Mở game như bình thường.
6. App sẽ sao lưu và xoá file logo VNG khi game chạy.
7. Thoát game bình thường.
8. App sẽ khôi phục file VNG gốc từ backup.

## Khôi Phục Thủ Công

Dùng nút `Khôi phục` ở trang chính hoặc menu dưới khay hệ thống nếu muốn trả file gốc ngay.

Nên khôi phục trước khi:

- repair game,
- update game,
- xoá thư mục tool,
- di chuyển thư mục backup.

## Giới Hạn Bản Public

Repo này chỉ mở phần thao tác file cục bộ. Các phần backend, key/license, cloud sync, máu đỏ, xác chết, đổi màu tâm và dữ liệu private không nằm trong source public.
