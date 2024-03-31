# 森海克斯修改版写频软件 说明

[![downloads](https://img.shields.io/github/downloads/SydneyOwl/shx8x00-freq-writer-enhanced/total)](https://github.com/SydneyOwl/shx8x00-freq-writer-enhanced/releases?style=for-the-badge)
[![downloads@latest](https://img.shields.io/github/downloads/SydneyOwl/shx8x00-freq-writer-enhanced/latest/total)](https://github.com/SydneyOwl/shx8x00-freq-writer-enhanced/releases/latest?style=for-the-badge)
![version](https://img.shields.io/github/v/tag/sydneyowl/shx8x00-freq-writer-enhanced?label=version&style=flat-square?style=for-the-badge)

![gt12](https://github.com/sydneyowl/senhaix-freq-writer-enhanced/actions/workflows/gt12.yml/badge.svg)
![shx8x00_nobt](https://github.com/sydneyowl/senhaix-freq-writer-enhanced/actions/workflows/shx8x00_nobt.yml/badge.svg)
![shx8x00](https://github.com/sydneyowl/senhaix-freq-writer-enhanced/actions/workflows/shx8x00.yml/badge.svg)

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

如果您需要同时克隆卫星数据，请使用`git clone --recursive`。

您可以`data/satellite_data/data`中查看目前版本默认使用的卫星数据。

## 其他

软件还在开发中，尚不稳定，欢迎提出 issues 和 pr!

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
