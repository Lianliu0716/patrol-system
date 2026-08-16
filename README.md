# Scene-Verification Patrol Check-In System

> **Undergraduate capstone project** — *An Intelligent Patrol System Based on Dynamic Scene Verification.*

A patrol check-in system that replaces "tap a tag" with **"photograph the scene and match it"**, so that a
check-in proves the guard was physically at the location — not just that a tag was scanned.

<!-- Screenshots go here:
<p align="center">
  <img src="docs/demo.png" alt="Demo — capture, match, and advance the patrol progress" width="480">
</p>
-->

## The problem

Electronic patrol check-in is normally done with NFC tags or QR codes. Both prove only that *something was
scanned*: a tag can be detached and carried away, a QR code can be photographed and re-used elsewhere.
Neither proves presence.

This system requires the guard to photograph a designated scene at each patrol point. The app matches that
photo against a reference image on the spot, and only advances the patrol when the match passes. The scene
is part of the environment — it cannot be carried away or re-photographed off-site.

## How it works

```
patrol point
   │
   ▼  Xamarin.Forms camera      capture the designated scene
   ▼  Firebase Storage          upload the shot, fetch the reference image
   ▼  OpenCV template matching  grayscale TM_CCOEFF_NORMED, per-point threshold
   ▼  Xamarin.Essentials        GPS coordinates captured alongside the match
   ▼  Realtime Database         write the check-in, advance the progress bar
```

### Two independent checks

Passing requires **both** signals, which is what makes the check-in hard to fake:

| Signal | What it proves | Failure mode it closes |
|---|---|---|
| Scene match | The camera is pointed at *this* scene | A tag carried to another location |
| GPS fix | The device is at *this* coordinate | A photo of a photo, taken elsewhere |

Neither alone is sufficient: a similar-looking scene elsewhere passes the visual check but fails GPS; a
spoofed location passes GPS but has no matching scene to photograph.

### Matching details

Implemented in `app/App1/Views/CameraViewPage.xaml.cs` with OpenCV for Android:

- **Normalized cross-correlation** — both images are converted to grayscale and compared with
  `Imgproc.MatchTemplate` using `TM_CCOEFF_NORMED`; the peak response decides the match. The normalized
  variant was chosen to reduce sensitivity to overall brightness changes.
- **Size and orientation fallbacks** — a field photo rarely matches the reference dimensions. If the
  reference is larger than the capture, the two are swapped; if only one dimension overflows
  (portrait vs. landscape), the image is rotated 90° and retried; only then is the pair rejected.
- **Per-point thresholds** — feature strength varies a lot between patrol points, so thresholds are set
  per target (0.30 / 0.35 / 0.45) rather than as a single global value.
- **Visual feedback** — on a successful match the app draws a rectangle over the matched region, so the
  guard can see what the system actually recognised.

## What is my own work

This was a team capstone project.

| Part | Directory | Mine? |
|---|---|---|
| **Mobile app** | `app/` | ✅ original — camera flow, patrol sequencing, GPS capture, custom step progress bar |
| **Web admin console** | `web/` | ✅ original |
| **Backend API + Firebase integration** | `server/` | 🤝 roughly half — shared with a teammate |
| **Scene-matching algorithm** | `app/App1/Views/CameraViewPage.xaml.cs` | 🤝 minor — mostly a teammate's work; I integrated it into the app flow |

No Firebase keys or project identifiers are included in this repository — see [Setup](#setup).

## Repository layout

```
.
├── app/                    ⭐ Xamarin.Forms client (Android / iOS / UWP)
│   └── App1/
│       ├── Views/CameraViewPage.xaml.cs   capture + scene matching
│       ├── Page1.xaml.cs                  patrol point list, GPS fix
│       ├── Finish.xaml.cs                 completion screen
│       ├── StepProgressBarControl.cs      custom step progress bar
│       └── Transpoter.cs                  cross-page state
├── web/                    ⭐ admin console (Firebase JS SDK); index_3.html is the latest
├── server/                 Node.js + Express API (patrol data CRUD)
└── *.example               config templates — real keys are gitignored
```

## Setup

The repository contains **no Firebase keys or project identifiers**. Bring your own Firebase project
(Realtime Database + Storage) and fill in three places.

**1. Mobile client**

```bash
cp app/App1/FirebaseConfig.cs.example app/App1/FirebaseConfig.cs
```

Set `DatabaseUrl` and `StorageBucket`. Reference images live in Storage under
`patrol_dataset/<point>/<id>.jpg`.

**2. Backend**

```bash
cp server/.env.example server/.env
npm install
```

Fill in the Firebase web config values (Firebase Console → Project settings → General).

**3. Web console**

```bash
cp web/.firebaserc.example web/.firebaserc
```

Also replace `YOUR_API_KEY`, `YOUR-PROJECT-ID` and `YOUR_SENDER_ID` in the HTML files under `web/` and in
`public/init-firebase.js`.

> A Firebase web `apiKey` is *designed* to be visible in client source — it is not a password. Access
> control belongs in Firebase Security Rules, not in hiding the key.

## Limitations

Stated plainly, because they are also the obvious next steps:

- **Template matching is sensitive to viewpoint and lighting.** Large angle offsets or a day/night change
  drop the correlation score noticeably. Feature-based matching (ORB / SIFT) or a lightweight learned
  model would be more robust.
- **Thresholds are hand-tuned.** The three values were chosen empirically and do not scale as patrol
  points are added; they should be derived from data.
- **Matching runs on-device.** Good for offline use and for not shipping raw photos to a server, but
  bounded by phone compute.

## Notes

- Built in 2022–2023. Xamarin.Forms has since been superseded by .NET MAUI.
- `web/` contains several `index*.html` iterations; `index_3.html` is the most complete.
