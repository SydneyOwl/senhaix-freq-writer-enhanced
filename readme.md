# 森海克斯修改版写频软件 说明

[![downloads](https://img.shields.io/github/downloads/SydneyOwl/shx8x00-freq-writer-enhanced/total)](https://github.com/SydneyOwl/shx8x00-freq-writer-enhanced/releases?style=for-the-badge)
[![downloads@latest](https://img.shields.io/github/downloads/SydneyOwl/shx8x00-freq-writer-enhanced/latest/total)](https://github.com/SydneyOwl/shx8x00-freq-writer-enhanced/releases/latest?style=for-the-badge)
![version](https://img.shields.io/github/v/tag/sydneyowl/shx8x00-freq-writer-enhanced?label=version&style=flat-square?style=for-the-badge)
![senhaix](https://github.com/sydneyowl/senhaix-freq-writer-enhanced/actions/workflows/build.yml/badge.svg)

## 简介

森海克斯8800/8600：[森海克斯8800/8600写频软件简介](./shx8x00/readme.md)

森海克斯GT-12：[GT12写频软件简介](./GT12/readme.md)

## 编译指引

如果需要自行编译，只需要分别对 `SHX8800` 和 `SHX8800_nobt` 进行编译即可。

|型号|编译要求|
|---|---|
|`SHX8800`|.net4.6.2, win10+|
|`SHX8800_nobt`|.net2.0|
|`gt12`|.net4.6.2, win8+|

您也可以参考`.github/workflows/build.yml`进行编译。

如果您需要同时克隆卫星数据，请使用`--recursive`。

## 其他

软件还在开发中，尚不稳定，欢迎提出 issues 和 pr!

卫星频率数据来源于[amateur-satellite-database](https://github.com/palewire/amateur-satellite-database)，参考了[业余无线电 FM 卫星频率表](https://forum.hamcq.cn/d/351)进行了多普勒修正。另外，请注意以下几点：

+ 除亚音外，所有频率单位均为 MHz（兆赫兹）。
+ 针对 FM 卫星，默认按照 U 段 ±10KHz，V 段 ±5KHz 生成。
+ SO-50 卫星的 OPEN 阶段为转发器激活之用。若转发器已激活，可跳过本阶段。两个 OPEN 分别对应 SO-50 出现和过顶，您可根据需要发射激活亚音。**（本软件未生成OPEN阶段）**
+ CAS-3H 和 PO-101 按照时间表计划开启转发器，请关注官方发布的时间表以及开机计划。
+ ISS-FM 转发器的开启请关注最新动态，在遇到 SSTV 活动、宇航员出舱活动等情况下可能关闭。
+ AO-91 卫星目前因电池故障，只能在日光下提供转发或工作状态不稳定，可能突然关机。
中国空间站（CSS）已完成业余载荷频率协调，但截至目前，CSS 的业余载荷还并未投入使用，请关注最新消息。
+ UVSQ-SAT 卫星目前以科研任务为主，何时开放转发器也请关注最新消息。
+ Tevel 系列卫星较多，且都为同一参数。因此是按计划，在部分时间开启部分卫星的转发器，请关注最新信息以了解哪些卫星的转发器在何时处于可用状态。
+ 本表中亚音频率的单位为 Hz（赫兹）。若生成的数据亚音标注为OFF，表明此卫星转发器无需亚音即可使用。
+ *以上几点来源：https://forum.hamcq.cn/d/351

shx8x00软件原理:见 [ble-connector](https://github.com/SydneyOwl/shx8800-ble-connector) 以及 [config-editor](https://github.com/SydneyOwl/shx8800-config-editor)

## 免责声明

- 本软件仅供技术交流和个人学习使用。任何个人或组织在使用本软件时必须**遵守中华人民共和国相关法律法规及无线电管理条例**。
- 如因使用本软件造成数据损失（理论上写频操作无此问题），**作者不承担任何法律责任**。

## 版本日志

`v0.1.a` 加入了蓝牙写频和便捷的信道更改。

`v0.1.0` 修复了即使蓝牙已连接也可能提示串口未连接的问题；修正了删除或清空信道时单元格未清空的问题。

`v0.1.1` 修复了 DataGridViewX 报错问题以及集成（实验性）开机画面修改。

`v0.1.2` 增加操作指引以及蓝牙连接状态指示。

`v0.1.3` 修复了信道操作后，会自动跳回 0 信道，还得拖动滚动条下去继续操作的问题。

`v0.1.4` 优化 UI，加入 GT12写频软件。

`v0.2.0` 修复蓝牙写频结束设备断开后，重新连接设备时搜索不到设备的问题/升级dotnet版本到4.6.2 LTS/更新所用依赖版本

`v0.2.1` 加入打星助手，更换了“关于”窗体

## 许可证

```markdown
This is free and unencumbered software released into the public domain.

Anyone is free to copy, modify, publish, use, compile, sell, or
distribute this software, either in source code form or as a compiled
binary, for any purpose, commercial or non-commercial, and by any
means.

In jurisdictions that recognize copyright laws, the author or authors
of this software dedicate any and all copyright interest in the
software to the public domain. We make this dedication for the benefit
of the public at large and to the detriment of our heirs and
successors. We intend this dedication to be an overt act of
relinquishment in perpetuity of all present and future rights to this
software under copyright law.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.
IN NO EVENT SHALL THE AUTHORS BE LIABLE FOR ANY CLAIM, DAMAGES OR
OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE,
ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
OTHER DEALINGS IN THE SOFTWARE.

For more information, please refer to <https://unlicense.org>
```
