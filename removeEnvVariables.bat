@ECHO OFF

REG delete “HKCU\Environment” /F /V “VENDOR1_USERNAME”
REG delete “HKCU\Environment” /F /V “VENDOR1_PASSWORD”
REG delete “HKCU\Environment” /F /V “VENDOR2_USERNAME”
REG delete “HKCU\Environment” /F /V “VENDOR2_PASSWORD”
REG delete “HKCU\Environment” /F /V “VENDOR3_USERNAME”
REG delete “HKCU\Environment” /F /V “VENDOR3_PASSWORD”