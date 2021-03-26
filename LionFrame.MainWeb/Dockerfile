FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base

RUN apt-get update -y && apt-get install -y libgdiplus && apt-get install -y libc6-dev
RUN apt-get clean && ln -s /usr/lib/libgdiplus.so /usr/lib/gdiplus.dll

WORKDIR /lionframe
COPY . /lionframe

EXPOSE 5000

ENV TZ=Asia/Shanghai
RUN ln -snf /usr/share/zoneinfo/$TZ /etc/localtime && echo $TZ > /etc/timezone

ENTRYPOINT ["dotnet", "LionFrame.MainWeb.dll"]