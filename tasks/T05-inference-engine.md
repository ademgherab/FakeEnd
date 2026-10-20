# T05 — Inference engine (sample JSON → schema)

- **Milestone:** M1
- **Depends on:** T04
- **Spec refs:** §5.2, §5.3, §5.4, §6 (all), §7.1, §10.3 (#2)

## Goal

Turn any valid JSON sample into a valid `Schema`, with deterministic warnings and suggestions, without persisting or logging the sample.

## Scope

- `SampleInferrer` in `MockApi.Core`.
- Limits enforcement (§6.1).
- Array merging, type conflict resolution, enum detection.
- Generator selection: value pattern → name rules → type fallback.
- `wrapInList` and pagination suggestions.

## Implementation checklist

- [ ] Parse with `System.Text.Json` `JsonElement`; integer vs number decided from raw text (`1` vs `1.0` / `1e3`).
- [ ] Reject root scalar/`null` with `422 sample_invalid`; reject missing `sample` with `400 validation_failed`.
- [ ] Limits: 100 KB serialized sample, depth 10, 100 fields/object, 2000 nodes → `422 sample_too_complex` (or `413` when the HTTP body exceeds 256 KB).
- [ ] Arrays: merge items into one `item` node; object field union in first-seen order; missing fields → `omitProbability = k/n`; null present fields → `nullProbability = k/n`.
- [ ] Conflicting types: most frequent non-null type wins; integer + number → number; emit `mixed_types` warning at the correct JSON path.
- [ ] Empty array → `null` leaf item + `empty_array` warning; nested array `length = { min: 1, max: max(3, n) }`.
- [ ] Generator selection order A → B → C exactly as §6.2 step 4.
- [ ] Implement all 35 name rules in §6.3, including parent hints from the parent field name or endpoint path and the key tokenizer (`createdAt` → `created, at`; `user_email` → `user, email`).
- [ ] Type fallbacks per §6.4, including precision from sample decimals (max 6) and `words` count ±1 clamped 1..6.
- [ ] Enum from arrays: 2–10 distinct string values → `enum`; emit `enum_from_sample`.
- [ ] Numeric-looking strings keep `type: "string"` but may use numeric generators (e.g. `price`), with the generator formatted to string output.
- [ ] `wrapInList`: wrap a non-array root in an array node; if the root is already an array, do nothing.
- [ ] Suggestions: `list.path` = JSON Pointer to the only array field of a root object; `pagination.bindings` auto-detected from the envelope names in §6.2 step 7.
- [ ] Never store/log the sample; only enum distinct values survive (§5.4).

## Acceptance criteria

- [ ] The §5.2 sample produces the exact schema shown (same generator names and params).
- [ ] An array of 100 objects with 3 missing and 2 null fields yields `omitProbability`/`nullProbability` of 0.03/0.02 respectively.
- [ ] Every emitted schema passes T04 validation.
- [ ] Inference is in-memory only: no DB writes, no sample content in logs.
- [ ] `wrapInList` produces a root array whose `item` is the inferred root object.

## Tests

- Golden-file tests for the spec sample, a nested order example, a paginated envelope, and an empty array.
- Property test: for random JSON within limits, inference output always validates.
- Name-rule table tests: one case per rule 1–35, plus ambiguous keys.
- Privacy test: canary string in the sample never appears in logs or storage.
- Limit tests: exactly-at and one-over for each §6.1 limit.

## Notes

- Keep the inferrer free of `Faker`; it only classifies and selects generators.
- Do not invent `key_order_unspecified`; §6.5 says it is unused in v1.
