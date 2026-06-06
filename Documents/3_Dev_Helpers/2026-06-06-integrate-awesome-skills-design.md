# Design Specification: Integrate Antigravity Awesome Skills

*   **Date**: 2026-06-06
*   **Status**: COMPLETED

---

## 1. Goal

Consolidate custom agent skills, rules, and workflows from the [sickn33/antigravity-awesome-skills](https://github.com/sickn33/antigravity-awesome-skills) repository into the main `WebIeltsFree` project workspace to enhance the agentic capabilities of the workspace.

---

## 2. Requirements & Constraints

1.  **Clone Location**: Clone the `antigravity-awesome-skills` repository on the E drive at:
    `E:\University\PTUDW\antigravity-awesome-skills`
2.  **Target Destination**: The files must be copied into the `.agents/` folder of the `WebIeltsFree` project root:
    *   `skills/` ➔ `WebIeltsFree/.agents/skills/`
    *   `rules/` ➔ `WebIeltsFree/.agents/rules/`
    *   `workflow/` ➔ `WebIeltsFree/.agents/workflows/`
3.  **Conflict Resolution**: If a skill folder already exists in the target destination, it **must be skipped** to preserve local custom configurations (e.g. `find-skills`, `frontend-design`, `ui-ux-pro-max`, `understand`, etc.).
4.  **No Unapproved Changes**: No modifications or copies will be performed in the active codebase without explicit user permission.

---

## 3. Proposed Execution Method (Scripted PowerShell)

We will use a PowerShell script to automate the cloning, validation, and copy processes:

```powershell
# 1. Clone the repository
git clone https://github.com/sickn33/antigravity-awesome-skills.git "E:\University\PTUDW\antigravity-awesome-skills"

# 2. Define source and destination folders
$src = "E:\University\PTUDW\antigravity-awesome-skills"
$dest = "e:\University\STKN\WebIeltsFree\.agents"

# 3. Create destination subfolders if they do not exist
New-Item -ItemType Directory -Path "$dest\skills", "$dest\rules", "$dest\workflows" -Force

# 4. Copy rules and workflows (overwrite allowed since they are new)
if (Test-Path "$src\rules") {
    Copy-Item -Path "$src\rules\*" -Destination "$dest\rules\" -Recurse -Force
}
if (Test-Path "$src\workflow") {
    Copy-Item -Path "$src\workflow\*" -Destination "$dest\workflows\" -Recurse -Force
}

# 5. Copy skills with conflict check (skip existing)
if (Test-Path "$src\skills") {
    Get-ChildItem -Path "$src\skills" -Directory | ForEach-Object {
        $skillName = $_.Name
        $targetPath = Join-Path "$dest\skills" $skillName
        if (Test-Path $targetPath) {
            Write-Host "Skipping existing skill: $skillName" -ForegroundColor Yellow
        } else {
            Copy-Item -Path $_.FullName -Destination "$dest\skills\" -Recurse -Force
            Write-Host "Copied skill: $skillName" -ForegroundColor Green
        }
    }
}
```

---

## 4. Verification Plan

1.  **Clone Verification**: Check that `E:\University\PTUDW\antigravity-awesome-skills` exists and is populated.
2.  **Preservation Verification**: Check that `WebIeltsFree/.agents/skills/find-skills/` and other pre-existing local skills remain unmodified.
3.  **Copy Verification**: Check that new folders in `.agents/rules/` and `.agents/workflows/` are successfully created and populated.

---

## 5. Actual Execution & Cleanup

1.  **Cloning & Copying**:
    - The repository `antigravity-awesome-skills` was successfully cloned and all skills, rules, and workflows copied without overwriting the pre-existing custom files.
2.  **Lập kế hoạch & Phê duyệt**:
    - Kế hoạch dọn dẹp thư mục skills đã được thiết lập tại [implementation_plan.md](file:///C:/Users/ADMIN/.gemini/antigravity-ide/brain/b862a76d-764a-4821-84a8-9cb82c33e5c3/implementation_plan.md).
    - Ý kiến đóng góp của người dùng (giữ lại các framework Frontend như React/Next.js/Svelte/Tailwind và công cụ Media/AI như fal.ai/Pipecat/VideoDB/Vizcom) được cập nhật chi tiết.
3.  **Dọn dẹp (Cleanup)**:
    - Tổng cộng **130 thư mục** skills không phù hợp (như Elixir, Kotlin, Unity, Django, zendesk, zoom, và các cấu hình CI/CD khác...) được xóa bỏ an toàn thông qua script tự động.
    - Tổng cộng **324 thư mục** skills được giữ lại (bao gồm cả các framework Frontend và các công cụ AI hỗ trợ).
4.  **Xác minh & Kết quả**:
    - Xác minh số lượng thư mục còn lại đạt chuẩn: **324** folders.
    - 13 kỹ năng hệ thống cốt lõi và các kỹ năng tùy biến (`ui-ux-pro-max`, `understand-explain`, `design-taste-frontend`) hoạt động nguyên vẹn.
    - Xóa sạch toàn bộ các script phụ trợ trong thư mục `scratch/` để duy trì sự sạch sẽ của workspace.
    - Kết quả chi tiết được tổng hợp tại [walkthrough.md](file:///C:/Users/ADMIN/.gemini/antigravity-ide/brain/b862a76d-764a-4821-84a8-9cb82c33e5c3/walkthrough.md).

