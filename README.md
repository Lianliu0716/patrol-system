# 基於動態景物核對之智慧巡邏系統

大學專題。透過行動裝置在巡邏點拍照，與資料庫中的基準影像比對，確認巡邏人員確實抵達現場並記錄巡邏進度。

## 專案結構

| 目錄 | 說明 | 技術 |
|---|---|---|
| `app/` | 行動端 App，負責拍照、比對流程、巡邏進度顯示 | Xamarin.Forms（Android / iOS / UWP） |
| `server/` | 後端 API，學生資料的 CRUD | Node.js + Express |
| `web/` | 網頁端管理介面 | HTML + Firebase JS SDK |

三個部分共用同一個 Firebase 專案（Realtime Database + Storage）。

### `app/` 主要檔案

- `Views/CameraViewPage.xaml.cs` — 相機擷取與影像比對主流程
- `Page1.xaml.cs` — 巡邏點清單
- `Finish.xaml.cs` — 巡邏完成頁
- `Transpoter.cs` — 資料傳輸
- `StepProgressBarControl.cs` — 自製步驟進度條元件

## 設定

這個 repo **不包含任何 Firebase 金鑰或專案識別碼**。要執行的話需自備 Firebase 專案，並填入以下三處設定：

### 1. 行動端

```
cp app/App1/FirebaseConfig.cs.example app/App1/FirebaseConfig.cs
```

填入 `DatabaseUrl` 與 `StorageBucket`。

### 2. 後端

```
cp server/.env.example server/.env
npm install
```

填入 Firebase web config 的各欄位（Firebase Console → 專案設定 → 一般）。

### 3. 網頁端

```
cp web/.firebaserc.example web/.firebaserc
```

另外把 `web/` 底下 HTML 與 `public/init-firebase.js` 中的 `YOUR_API_KEY`、`YOUR-PROJECT-ID`、`YOUR_SENDER_ID` 換成實際值。

> **注意**：Firebase 的 web apiKey 設計上就會出現在前端原始碼中，它不是密碼。實際的存取控制請設定 Firebase 的安全性規則（Security Rules），不要依賴隱藏 apiKey。

## 備註

- 原始開發於 2022–2023 年，Xamarin.Forms 已由 .NET MAUI 取代。
- `web/` 底下數個 `index*.html` 為開發過程中的不同版本，`index_3.html` 為最新且功能最完整者。
