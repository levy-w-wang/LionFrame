[toc]


## 多个联合部署到Docker中



多应用需要各自互相访问，为了提高访问速度，肯定是使用内网访问，因此得先创建内网网络

### 创建内网

docker network ls     **查看当前有哪些网络**

docker network rm net-name     **删除某个网络**

docker network create --subnet=192.168.0.0/24 --gateway=192.168.0.1 lion-net    **创建一个网络，指定子网地址**

docker network create --driver bridge lion-net   **创建一个网络，指定bridge模式**

docker network inspect lion-net    **查看某个网络下，连接的容器**

docker run  -d --name busybox2 -p 8000:80  --net lion-net  --ip 192.168.0.3 busybox **后台运行 指定容器名称 指定8000映射容器中的80端口  指定网络名称 指定内网ip  镜像名称**

注意`--network`标志。您只能在`docker run`命令期间连接到一个网络，因此您`docker network connect`以后也需要使用它 来连接 `容器名称` 到`网络名称` 网络。

## 涉及的技术

rabbitMQ  netcore3.1  vue redis mysql 

### 创建RabbitMQ

拉取镜像

```linux
docker pull rabbitmq:management
```

运行镜像

```linux
docker run -d -p 5672:5672 -p 15672:15672 --net lion-net --ip 192.168.0.2 --restart unless-stopped --name rabbitmq rabbitmq:management
指定IP 方便应用内网联网  不用绕外网链接
```

### 拉取Net Core3.1运行环境

```linux
docker pull mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim
```

### 修改应用相关参数

MYSQL

Redis

RabbitMQ

### 发布到docker

把生成文件移动到服务器

打包镜像

```linux
docker build -t lionframe:v1 .
```

运行镜像

```linux
docker run -d -p 5000:5000 --net lion-net --ip 192.168.0.3 --restart unless-stopped --name lionframe  lionframe:v1
-d 后台运行
-p 服务器端口映射容器端口
--net 指定网络名
--ip 指定该网络下IP
--name 命名
--restart always     docker daemon会无限尝试重启退出的容器（无论以什么退出码退出）。手动停止容器后，容器策略不再生效。除非重启docker daemon
--restart unless-stopped     与always类似，区别在于手动停止容器后，就算重启docker daemon，容器策略也不再生效。

```

```linux
docker logs 容器ID //查看日志
docker rmi 镜像ID // 删除镜像
docker stop 容器ID //暂停容器
docker start 容器ID //启动容器
docker rm 容器ID  // 删除容器
```

由于这里只有MQ是在容器中部署的，因此没有使用docker-compose。

在多个应用同时部署需要指定网络或挂载数据的情况时，可使用docker-compose 以避免出现配置错误，且发布起来更方便