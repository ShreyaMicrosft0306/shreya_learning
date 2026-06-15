@echo off
cd /d "c:\personal_workspace\DSA-CSharp"
git add .
git commit -m "Add all DSA solutions and documentation"
git branch -M main
git remote remove origin
git remote add origin https://github.com/ShreyaMicrosft0306/shreya_learning.git
git push -u origin main --force
pause
