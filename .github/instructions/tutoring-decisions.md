# Tutoring Decisions Log

Purpose:
- Append-only trace of plan-time tutoring decisions and learning opportunities.
- Keep entries understandable for the learner's current coding proficiency.

Entry format (append for each decision gate):
- Timestamp (local)
- Prompt Problem Summary
- Options Explained
- User Decision
- Recommendation Rationale
- Rationale Against Non-Selected Option

## 2026-04-03 23:05
- Prompt Problem Summary:
  - The user asked to restore optional +1/+2 proficiency stretch behavior, require explicit plan-mode acknowledgment before using stretch, and persist decision rationale in an append-only log.
- Options Explained:
  - Baseline option:
    - Keep all plan and implementation concepts at or below current proficiency.
    - Benefit: predictable cognitive load, lower confusion risk, easier stage validation.
  - Stretch option (+1/+2 with explicit consent):
    - Allow selected concepts up to one or two levels above current proficiency only when the plan asks first and the user explicitly approves.
    - Benefit: targeted growth opportunities while preserving learner control.
- User Decision:
  - Adopt stretch-capable policy with explicit plan-mode consent and append-only decision logging.
  - Keep decision gate in planning flow; agent mode should consume approved decision and avoid re-asking.
- Recommendation Rationale:
  - This balances safety and growth: default stays level-appropriate, and stretch is only used when learner-approved with clear tradeoffs.
  - It improves consistency across plan and implementation without forcing advanced concepts unexpectedly.
- Rationale Against Non-Selected Option:
  - Rejecting stretch entirely would reduce intentional learning progression opportunities for motivated learners.
  - Stretch without explicit consent would increase confusion risk and break learner control.
