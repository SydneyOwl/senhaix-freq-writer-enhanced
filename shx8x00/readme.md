# Shx8x00写频软件 简介

**注意：该简介仅适用于森海克斯8600/8800; GT12见 [GT12写频简介](../GT12/readme.md)**

该软件在原官方写频软件的基础上简化了对信道的操作，同时增加了蓝牙写频功能。只要打开手台蓝牙功能，不需写频线即可写频。

<img src="./readme_shx8x00/image-20240118150904988.png" alt="image-20240118150904988" style="zoom: 67%;" />

<img src="./readme_shx8x00/image-20240113121213939.png" alt="image-20240113121213939" style="zoom:50%;" />

<img src="./readme_shx8x00/SAT.png" alt="SAT" style="zoom:75%;" />

## 功能

**使用视频：[软件使用示范](https://www.bilibili.com/video/BV1Et4y1R7ax/)**

软件目前支持的功能：

  | 信道操作                     |     Supported      |
  | ---------------------------- | :----------------: |
  | 上下拖拽调整信道顺序         | :white_check_mark: |
  | 一键删除空信道               | :white_check_mark: |
  | 撤回                         | :white_check_mark: |
  | 清空指定信道（右键）         | :white_check_mark: |
  | 指定信道后插入空信道（右键） | :white_check_mark: |
  | 删除指定信道（右键）         | :white_check_mark: |
  | 复制（右键）                 | :white_check_mark: |
  | 剪切（右键）                 | :white_check_mark: |
  | 粘贴（右键）                 | :white_check_mark: |
  | 以及所有原有的功能           | :white_check_mark: |

  | 蓝牙写频                                      |     Supported      |
  | --------------------------------------------- | :----------------: |
  | 不过滤 `ssid`                                 | :white_check_mark: |
  | 不过滤 `rssi < -80` 的信号                    | :white_check_mark: |
  | 连接森海克斯8800或后续成功加入蓝牙芯片的 8600 | :white_check_mark: |
  | 所有写频线写频支持的功能                      | :white_check_mark: |

  | 其他功能         |          Supported           |
  |--------------| :--------------------------: |
  | 蓝牙或写频线修改开机画面 | `Experimental` :interrobang: |
  | 打星助手         | :white_check_mark: |

> [!CAUTION]
>
> 使用蓝牙修改开机画面可能造成写入失败，或写入画面不完整。遇到此情况请使用写频线重写。

## 注意事项

release中带有 `bluetooth` 字样的软件带有蓝牙写频功能，由于在 .net461基础上开发且使用了 BLE，**故该版本至少在 windows10及以上系统，且具备蓝牙硬件的电脑上方可使用**。

没有 `bluetooth` 字样的版本使用 .net20开发，不具备蓝牙功能，**在 windows7 及以上系统即可运行**。