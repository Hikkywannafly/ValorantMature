# Cơ Chế Hoạt Động

[Về README](../../README.md) · [Discord hỗ trợ](https://discord.gg/DXX4x5TQRq)

## Luồng Tổng Quát

ValorantMature Public hoạt động theo luồng cục bộ:

```text
Mở app
  -> tìm thư mục Paks của VALORANT
  -> theo dõi tiến trình game
  -> game mở
  -> sao lưu file logo VNG
  -> xoá file logo VNG
  -> game đóng
  -> khôi phục file gốc
```

## Tìm Thư Mục Game

App thử tìm thư mục `Paks` bằng metadata cài đặt local và các đường dẫn cài phổ biến. Nếu không tìm được, người dùng có thể chọn thủ công.

Các kiểu thư mục có thể chọn:

- thư mục gốc cài VALORANT,
- `ShooterGame\Content\Paks`,
- thư mục đang chứa trực tiếp file `.pak`.

## Sao Lưu

Trước khi xoá file logo VNG, app copy file gốc vào thư mục `backup`. Khi game đóng hoặc người dùng bấm khôi phục, app copy file từ `backup` trở lại thư mục game.

Không nên xoá hoặc di chuyển thư mục `backup`, vì đây là nguồn để restore.

## Thao Tác File

Bản public chỉ xử lý danh sách file logo VNG được định nghĩa trong engine local. Repo public không phát hành dữ liệu game private.

Luồng file rất đơn giản:

- kiểm tra file có tồn tại không,
- copy sang backup,
- xoá file trong thư mục game,
- restore từ backup khi cần.

## Theo Dõi Game

Monitor kiểm tra tiến trình VALORANT. Mỗi phiên game chỉ xử lý một lần. Khi tiến trình game không còn chạy, app khôi phục file gốc.

## Phần Không Có Trong Public

Các phần sau được giữ private:

- backend/license,
- gating tính năng premium,
- cloud sync,
- logic máu đỏ và xác chết,
- đổi profile tâm qua cloud,
- dữ liệu game private.
