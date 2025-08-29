# LAZY.CAPTCHA 配置说明

## CaptchaOptions 配置项详解

### 基本配置

| 配置项 | 类型 | 默认值 | 说明 |
|-------|------|--------|------|
| `CaptchaType` | int | 5 | 验证码类型: 1-数字, 2-字母, 3-数字+字母, 4-中文, 5-数字+字母+中文 |
| `CodeLength` | int | 4 | 验证码长度 |
| `ExpirySeconds` | int | 60 | 验证码过期时间(秒) |
| `IgnoreCase` | bool | true | 验证时是否忽略大小写 |
| `StorageKeyPrefix` | string | "" | 存储键前缀 |

### 频率限制配置 (RateLimit)

| 配置项 | 类型 | 默认值 | 说明 |
|-------|------|--------|------|
| `Enabled` | bool | false | 是否启用频率限制 |
| `WindowSeconds` | int | 60 | 时间窗口(秒) |
| `MaxRequests` | int | 5 | 最大请求数 |

### 图片选项 (ImageOption)

| 配置项 | 类型 | 默认值 | 说明 |
|-------|------|--------|------|
| `Animation` | bool | false | 是否生成GIF动画 |
| `FontSize` | int | 32 | 字体大小 |
| `Width` | int | 100 | 图片宽度 |
| `Height` | int | 40 | 图片高度 |
| `BubbleMinRadius` | int | 5 | 气泡最小半径 |
| `BubbleMaxRadius` | int | 10 | 气泡最大半径 |
| `BubbleCount` | int | 3 | 气泡数量 |
| `BubbleThickness` | double | 1.0 | 气泡线条粗细 |
| `InterferenceLineCount` | int | 3 | 干扰线数量 |
| `FontFamily` | string | "kaiti" | 字体族 |
| `FrameDelay` | int | 15 | GIF动画帧延迟(毫秒) |
| `BackgroundColor` | string | "#ffffff" | 背景颜色 |
| `ForegroundColors` | string | "" | 前景色(多个颜色用逗号分隔) |
| `Quality` | int | 100 | 图片质量(0-100) |
| `TextBold` | bool | false | 文字是否加粗 |