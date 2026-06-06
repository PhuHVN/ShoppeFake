# Docker Setup Guide for ShoppeFake

Hướng dẫn cài đặt và chạy ứng dụng ShoppeFake với Docker.

## Yêu cầu hệ thống

- Docker & Docker Compose
- .NET 8.0 SDK (cho development)
- **PostgreSQL 12+ đã được deploy và chạy ngoài Docker**

## Các file được tạo

1. **Dockerfile** - Build image cho .NET API
2. **docker-compose.yml** - Chỉ chứa API service (kết nối tới PostgreSQL ngoài Docker)
3. **.env.example** - Template cho environment variables
4. **.dockerignore** - Loại bỏ file không cần thiết khi build

## Cách sử dụng

### Bước 1: Cấu hình Environment Variables

```bash
# Sao chép .env.example thành .env
cp .env.example .env

# Chỉnh sửa .env để trỏ tới PostgreSQL đã deployed
# Ví dụ: DEFAULT_CONNECTION=Server=your-postgres-host;Port=5432;Database=ShoppeFakeDB;User Id=postgres;Password=your_password;
```

### Bước 2: Build và chạy API

```bash
# Build image và chạy API service
docker-compose up -d

# Xem logs
docker-compose logs -f

# Dừng services
docker-compose down
```

### Bước 3: Kiểm tra ứng dụng

- **API**: http://localhost:8080
- **Swagger/OpenAPI**: http://localhost:8080/swagger/index.html
- **PostgreSQL**: localhost:5432 (postgres user)

## Environment Variables

| Variable | Default | Mô tả |
|----------|---------|-------|
| `ASPNETCORE_ENVIRONMENT` | Development | ASP.NET Core environment (Development/Production) |
| `DefaultConnection` | - | PostgreSQL connection string |
| `DB_USER` | postgres | PostgreSQL username |
| `DB_PASSWORD` | YourPasswordHere123! | PostgreSQL password |
| `DB_PORT` | 5432 | PostgreSQL port |
| `JWT_SECRET_KEY` | - | JWT secret key (min 32 characters) |

## Services được tạo

### 1. API (.NET 8.0)
```
- Port: 8080
- Health Check: /swagger/index.html
- Kết nối: PostgreSQL ngoài Docker
- Không có dependencies khác

```bash
# Xem trạng thái containers
docker-compose ps

# Xem logs chi tiết
docker-compose logs -f api
docker-compose logs -f postgres

# Truy cập PostgreSQL từ container
docker exec -it shoppefake-postgres psql -U postgres -d ShoppeFakeDB

# Rebuild images sau khi thay đổi code
docker-compose build --no-
docker-compose ps

# Xem logs
docker-compose logs -f api

# Rebuild image sau khi thay đổi code
docker-compose build --no-cache
docker-compose up -d

# Xóa unused images
docker system prune -aProgram.cs để auto-migrate khi startup
```

## Troubleshooting

### Connection string không kết nối được
- Đảm bảo `DefaultConnection` sử dụng hostname `mssql` thay vì `localhost`
- Kiểm tra SQL Server container đang chạy: `dockpostgres` thay vì `localhost`
- Kiểm tra PostgreSQL container đang chạy: `docker-compose ps`

```bash
# Thay đổi port trong docker-compose.yml hoặc .env
API_PORT=8081
DB_PORT=1434
```

### Rebuild complete
```bash
docker-compose down -v
docker-compose build --no-cache
docker-compose up -d
```

## Production Deployment

Để deploy lên production:

1. Thay đổi `ASPNETCORE_ENVIRONMENT=Production`
2. Sử dụng strong JWT secret (min 32 characters)
3. Cấu hình HTTPS (thêm SSL certificates)
4. Sử dụng external database thay vì container
5. Cấu hình proper logging
6. Sử dụng environment variables từ secrets management

```yaml
# Example production docker-compose.yml snippet
environment:
  ASPNETCORE_ENVIRONMENT: Production
  Kestrel__Certificates__Default__Path: /https/cert.pfx
  Kestrel__Certificates__Default__Password: ${CERT_PASSWORD}
```

## Documentation

- [Docker Documentation](https://docs.docker.com/)
- [Docker Compose Documentation](https://docs.docker.com/compose/)
- [Microsoft .NET Container Images](https://github.com/dotnet/dotnet-docker)
- [SQL Server on Linux](https://docs.microsoft.com/en-us/sql/linux/sql-server-linux-overview)
