
# 🏦 Loan Management System – Dynamics 365 CRM (Plugins)

A mid-level Microsoft Dynamics 365 (Dataverse) development project implementing a financial Loan Management module with custom tables, plugins, and business automation.

---

# 📌 Project Overview

This solution demonstrates:

* Custom table design
* Business validation using Plugins
* EMI calculation logic
* Automated Approval creation
* Proper Plugin pipeline usage (Pre/Post stages)
* Post Image optimization
* Duplicate prevention logic
* Clean CRM architecture practices

---

# 🧱 Solution Components

## 🔹 1. Custom Tables

### 🏢 Loan Application (`lms_loanapplication`)

| Field           | Logical Name         | Type             |
| --------------- | -------------------- | ---------------- |
| Loan Number     | Primary (Autonumber) | Autonumber       |
| Customer        | lms_customer         | Lookup (Account) |
| Loan Amount     | lms_loanamount       | Currency         |
| Interest Rate   | lms_interestrate     | Decimal          |
| Tenure (Months) | lms_tenuremonths     | Whole Number     |
| EMI             | lms_emi              | Currency         |
| Credit Score    | lms_creditscore      | Whole Number     |
| Loan Status     | lms_loanstatus       | Choice           |

---

### 🧾 Loan Approval (`lms_loanapproval`)

| Field           | Logical Name       | Type                      |
| --------------- | ------------------ | ------------------------- |
| Approval Number | lms_approvalnumber | Autonumber                |
| Loan            | lms_loan           | Lookup (Loan Application) |
| Approval Level  | lms_approvallevel  | Choice                    |
| Approval Status | lms_approvalstatus | Choice                    |
| Approver        | lms_approver       | Lookup (System User)      |
| Comments        | lms_comments       | Text                      |

Relationship:

```
Loan Application (1) → Loan Approval (N)
```

---

# ⚙️ Plugin Architecture

Assembly Name:

```
LoanManagement.Plugins
```

Framework:

```
.NET Framework 4.6.2
```

Isolation Mode:

```
Sandbox
```

---

# 🔐 1️⃣ Credit Score Validation Plugin

Class:

```
LoanPreCreateValidation
```

Stage:

```
PreOperation
```

Message:

```
Create
```

### Business Rule

If:

```
Credit Score < 600
```

Then:

```
Block Loan Creation
```

Implementation:

* Uses `InvalidPluginExecutionException`
* Executes before record is saved
* Ensures business compliance

---

# 💰 2️⃣ EMI Calculation Plugin

Class:

```
LoanEmiCalculation
```

Stage:

```
PostOperation
```

Messages:

```
Create
Update
```

Filtering Attributes (Update):

```
lms_loanamount
lms_interestrate
lms_tenuremonths
```

### EMI Formula

[
EMI = \frac{P × R × (1+R)^N}{(1+R)^N - 1}
]

Where:

* P = Principal (Loan Amount)
* R = Monthly Interest Rate
* N = Tenure (Months)

### Technical Implementation

* Uses `PostEntityImages`
* Avoids unnecessary `Retrieve()`
* Uses Depth check to prevent infinite loops
* Updates EMI field after save

---

# 🏢 3️⃣ Automatic Loan Approval Creation

Class:

```
LoanApprovalCreation
```

Stage:

```
PostOperation
```

Messages:

```
Create
Update
```

Post Image:

```
lms_loanamount
```

Filtering Attributes:

```
lms_loanamount
```

### Business Rule

If:

```
Loan Amount > 500000
```

Then:

* Check if approval already exists
* Create Loan Approval record
* Set Status = Pending
* Set Level = Manager
* Assign Approver
* Prevent duplicate approval creation

---

# 🧠 Technical Best Practices Implemented

✔ Used Post Images instead of Retrieve
✔ Used Filtering Attributes for performance
✔ Depth check to avoid recursion
✔ Logical name consistency
✔ Version-controlled assembly update
✔ Duplicate prevention logic
✔ Clean separation of plugin classes

---

# 🧪 Test Scenarios Covered

### Credit Validation

* Credit Score < 600 → Blocked
* Credit Score ≥ 600 → Allowed

### EMI Calculation

* Create Loan → EMI auto-calculated
* Update Amount → EMI recalculated

### Approval Creation

* Loan < 500000 → No approval
* Loan > 500000 → Approval created
* Re-update → No duplicate approval

---

# 📂 Project Structure

```
LoanManagement.Plugins
│
├── LoanPreCreateValidation.cs
├── LoanEmiCalculation.cs
├── LoanApprovalCreation.cs
├── AssemblyInfo.cs
```

---

# 🚀 Skills Demonstrated

* Dynamics 365 Plugin Development
* Dataverse Customization
* Business Rule Enforcement
* QueryExpression usage
* EntityReference handling
* OptionSetValue handling
* Transaction Pipeline understanding
* Performance optimization using Images
* Real-world CRM architecture design

---

# 📈 Next Enhancements (Planned)

* Multi-level dynamic approval engine
* Async plugin for payment schedule generation
* Config-driven approval rules
* Loan Status auto-update based on approval
* CI/CD using Azure DevOps

---

# 👨‍💻 Author

Faizan
Microsoft Dynamics 365 CRM Developer
Focused on enterprise-grade CRM solutions and scalable plugin architecture.

---

# 🏁 Conclusion

This project demonstrates practical mid-level Dynamics 365 CRM development including business validation, automation, and architectural best practices aligned with enterprise standards.

---

