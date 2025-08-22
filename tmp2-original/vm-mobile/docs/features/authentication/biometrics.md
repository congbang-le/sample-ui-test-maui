# Biometric Authentication

## Overview

The biometric authentication system in VisitTracker enhances security by ensuring only authorized careworkers can start visits. It uses device fingerprint sensors to verify user identity and detect tampering with biometric data.

## Purpose

- **User Verification**: Ensures the person starting a visit is the authorized careworker.
- **Fraud Prevention**: Detects when fingerprints/biometrics have been modified or tampered with.
- **Security Compliance**: Helps meet regulatory requirements for care visit verification.

## How It Works

### Initial Registration

1. During onboarding, an employer ensures the careworker's fingerprints/biometrics are registered on the device.
2. The system takes a "snapshot" of the current biometric state.
3. This snapshot is used as a baseline for future verification.

### Visit Authentication Flow

1. When a careworker attempts to start a visit, the system requests biometric verification.
2. The system also checks if registered biometrics have been altered since initial setup.
3. Authentication results and tamper detection are included in visit reports.

## Technical Implementation

### Biometric Snapshot Service

This service provides platform-specific implementations for monitoring biometric integrity:

#### Key Methods

- **SnapshotBiometricState()**: Creates a secure key bound to current biometric data
- **ClearBiometricState()**: Removes the biometric key
- **HasBiometricStateChanged()**: Detects if biometrics were modified since registration

## Integration Points

- **Visit Start**: Biometric verification occurs in `SystemHelper.VerifyBiometric()` before visits begin
- **Visit Reporting**: Biometric status is included in visit reports through `VisitBiometricDto`

## Security Considerations

- The system detects if fingerprints/biometrics have been added, removed, or changed since registration
- Cryptographic keys are invalidated if biometric data changes
- Platform-specific security features are leveraged (Android KeyStore and iOS Keychain)

## References

- [SystemHelper.cs](../../../src/VisitTracker/Core/Helpers/SystemHelper.cs) - Biometric verification
- [BiometricSnapshotService (iOS)](../../../src/VisitTracker/Platforms/iOS/Services/BiometricSnapshotService.cs) - iOS implementation
- [BiometricSnapshotService (Android)](../../../src/VisitTracker/Platforms/Android/Services/BiometricSnapshotService.cs) - Android implementation