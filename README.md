## Struktur Proyek
Repositori ini berisi solusi .sln dengan struktur sebagai berikut:

- Clems.Application: Lapisan aplikasi yang berisi implementasi.
- Clems.Domain: Model domain dan entitas.
- Clems.Infrastructure: Implementasi akses data dan layanan eksternal.
- Clems.Web: Antarmuka pengguna berbasis web.
- SharedKernel: Kode bersama yang digunakan di seluruh proyek.

## Requirement
- .NET 9 
- Visual Studio 2022

## Running
```shell
git clone https://github.com/perdanaarta/laughing-giraffee.git perdanaarta
cd perdanaarta
dotnet run --project Clems.Web
```

