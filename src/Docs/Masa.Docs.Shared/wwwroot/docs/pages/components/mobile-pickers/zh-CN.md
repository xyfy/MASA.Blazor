---
title: Mobile pickers（移动端选择器）
desc: "专为移动设备设计的选择器。提供多个选项集合供用户选择，支持单列选择、多列选择和级联选择。"
related:
  - /components/mobile-picker-views
  - /components/mobile-date-time-views
  - /components/mobile-time-pickers
---


## 示例

### 属性

#### 级联

使用级联的 `Columns` 和 `ItemChildren` 字段可以实现选项级联的效果。

<!--alert:warning-->
级联选择的数据嵌套深度需要一致，如果某些选项没有子选项，则可以使用空字符串作为占位符。
<!--/alert:warning-->

<masa-example file="Examples.mobile_pickers.Cascade"></masa-example>

#### 自定义项高度

通过 `1Itemheight` 可以自定义选项的高度。目前只支持 `px` 。

<masa-example file="Examples.mobile_pickers.ItemHeight"></masa-example>