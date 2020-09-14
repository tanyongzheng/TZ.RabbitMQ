# TZ.RabbitMQ

[![nuget](https://img.shields.io/nuget/v/TZ.RabbitMQ.Client.svg?style=flat-square)](https://www.nuget.org/packages/TZ.RabbitMQ.Client) 
[![stats](https://img.shields.io/nuget/dt/TZ.RabbitMQ.Client.svg?style=flat-square)](https://www.nuget.org/stats/packages/TZ.RabbitMQ.Client?groupby=Version)
[![License](https://img.shields.io/badge/license-Apache2.0-blue.svg)](https://github.com/tanyongzheng/TZ.RabbitMQ.Client/blob/master/LICENSE)
![.NETStandard](https://img.shields.io/badge/.NETStandard-%3E%3D2.0-green.svg)

## 介绍
基于RabbitMQ.Client封装


主要功能：
1. 工作队列
2. 延时队列


## 使用说明

1. Install-Package TZ.RabbitMQ.Client

2. 注入服务：
```cs
    services.AddRabbitMQClient(Configuration);
```

3. 配置Redis
```js
    "RabbitMQClient": {
    "Host": "127.0.0.1",
    "Port": 5672,
    "VirtualHost": "/abc",
    "Username": "aaa",
    "Password": "bbb"
  }
```

4. 使用见项目Demo
