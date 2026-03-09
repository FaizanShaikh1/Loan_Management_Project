# 🚀 Feature: Shared Variables, Risk Level Processing & Bulk Import Support

## Overview

This feature enhances the **Loan Management System in Microsoft Dynamics 365 CRM** by introducing advanced plugin communication using **Shared Variables**, dynamic **Risk Level calculation**, and reliable **bulk data import support**.

The goal was to simulate real-world financial CRM scenarios where loan applications may be created individually or imported in bulk while still enforcing business logic such as risk evaluation, EMI calculation, and approval workflow.

---

# 🧩 Key Enhancements

## 1️⃣ Shared Variables Between Plugins

Implemented **SharedVariables** to allow communication between plugin steps within the same execution pipeline.

### Why it was needed

Previously each plugin worked independently. Using SharedVariables allows passing calculated data (Risk Level) from the validation plugin to downstream plugins.

### Implementation

Risk level is calculated in the **PreOperation plugin** and stored in the execution context.

Example:

```csharp
context.SharedVariables["RiskLevel"] = riskLevel;
```

This value is then consumed by the approval plugin.

---

# 📊 Risk Level Calculation

The system automatically determines loan risk based on **Credit Score**.

| Credit Score | Risk Level |
| ------------ | ---------- |
| < 650        | High       |
| 650 – 750    | Medium     |
| > 750        | Low        |

This risk level is used to update the **Loan Approval record**.

---

# 💰 EMI Auto Calculation

The **LoanEmiCalculation plugin** calculates the EMI based on:

* Loan Amount
* Interest Rate
* Tenure (Months)

Formula used:

[
EMI = \frac{P × R × (1+R)^N}{(1+R)^N - 1}
]

Where:

* **P** = Principal
* **R** = Monthly Interest Rate
* **N** = Tenure in months

EMI is automatically updated in the Loan Application record.

---

# 🏦 Automatic Loan Approval Creation

When the loan amount exceeds a defined threshold, an approval record is created.

### Rule

```
If Loan Amount > 500000
→ Create Loan Approval record
```

Approval record includes:

* Loan reference
* Approval Level
* Approval Status
* Approver
* Risk Level

---

# 📦 Bulk Data Import Support

This feature ensures the system works correctly when **large volumes of loan applications are imported**.

### Testing Scenario

More than **1000 loan records** were imported using CSV.

Plugins executed correctly for each record, including:

* Validation
* Risk calculation
* EMI generation
* Approval creation

---

# 🔁 Depth Handling Fix

During bulk imports, plugin execution depth may exceed normal values due to internal CRM operations.

To avoid unnecessary plugin termination while preventing infinite loops, depth validation was adjusted.

Previous condition:

```csharp
if (context.Depth > 1) return;
```

Updated condition:

```csharp
if (context.Depth > 3) return;
```

This ensures plugins still execute during bulk operations.

---

# 🧪 Testing Scenarios

### Manual Record Creation

Expected behavior:

* EMI calculated
* Risk level determined
* Approval created when required

---

### Bulk Import

Import file containing **1000+ loan records** was used.

Expected behavior:

| Condition            | Result           |
| -------------------- | ---------------- |
| Credit Score < 600   | Record rejected  |
| Credit Score ≥ 600   | Record created   |
| Loan Amount > 500000 | Approval created |
| Valid data           | EMI calculated   |

---

# 📂 Plugin Classes

| Plugin                  | Responsibility                                   |
| ----------------------- | ------------------------------------------------ |
| LoanPreCreateValidation | Validates credit score and determines risk level |
| LoanEmiCalculation      | Calculates EMI                                   |
| LoanApprovalCreation    | Creates or updates loan approval records         |

---

# ⚙️ Technologies Used

* Microsoft Dynamics 365 CRM
* Dataverse Plugins
* C#
* Plugin Registration Tool
* Git & GitHub
* CSV Data Import

---

# 🧠 Key Learnings

* Plugin pipeline execution
* SharedVariables usage
* Handling bulk data operations
* Preventing recursive plugin execution
* Git recovery and branch management

---

# 🔜 Future Improvements

Planned enhancements include:

* Multi-level approval workflow
* Payment schedule generation
* Custom API for EMI calculation
* CI/CD pipeline for plugin deployment

---

# 👨‍💻 Author

**Faizan Shaikh**
Microsoft Dynamics 365 CRM Developer

---

]
