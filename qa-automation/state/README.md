# Run state

- `last-success.json`: sprint commit tested by the last run that finished (exit 0 or 1); baseline of the next change scan.
- `latest.json`: status of the last run, whatever its outcome (also SKIPPED / errors).
- `history.jsonl`: one line per finished run (effective verdicts), used for the diff and flaky detection.

Written only by `scripts/run-weekly.ps1` and `scripts/diff-results.ps1`.
