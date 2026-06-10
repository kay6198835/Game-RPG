# World Cup Score Prediction Tracker

A standalone single-file web page for tracking World Cup score predictions among friends.

## Features

- List upcoming World Cup matches (seeded with placeholder openers; add your own via the form)
- Add participants with their predicted score for each match
- Enter the final result for a match; scoring is applied automatically:
  - Correct exact-score prediction: **+0 points**
  - Wrong prediction: **−1 point**
- Leaderboard ranks players by total points (least negative wins)
- All data persists in the browser via `localStorage` — no server required

## Usage

Open `index.html` in any modern browser. No build step, no dependencies.

## Notes

- This is a standalone utility and is intentionally isolated from the Unity project under `Assets/`.
- "Correct" means the exact score matches (e.g. predicting 2-1 when the result is 2-1).
- Use "Sửa lại kết quả" (edit result) on a finished match to correct a mistyped result; points recompute automatically.
