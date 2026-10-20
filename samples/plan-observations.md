# Actual plans (SQL Server 2022, server collation SQL_Latin1_General_CP1_CI_AS, 50000 rows)

| Case | Observed |
|---|---|
| ESC001:bad | rows=1 reads=219 ops=[Index Scan] seekOnEmail=False convertImplicit=True PAC(Seek)=True PAC(CE)=False rangeThroughConvert=False rangeMismatched=False |
| ESC001:good | rows=1 reads=3 ops=[Index Seek] seekOnEmail=True convertImplicit=False PAC(Seek)=False PAC(CE)=False rangeThroughConvert=False rangeMismatched=False |
| ESC002:bad | rows=1 reads=192 ops=[Index Scan] seekOnSku=False convertImplicit=True PAC(Seek)=True PAC(CE)=False rangeThroughConvert=False rangeMismatched=False |
| ESC002:good | rows=1 reads=2 ops=[Index Seek] seekOnSku=True convertImplicit=False PAC(Seek)=False PAC(CE)=False rangeThroughConvert=False rangeMismatched=False |
| ESC003:bad | rows=1 reads=2 ops=[Index Seek] seekOnReference=True convertImplicit=False PAC(Seek)=False PAC(CE)=False rangeThroughConvert=False rangeMismatched=False |
| ESC003:good | rows=1 reads=2 ops=[Index Seek] seekOnReference=True convertImplicit=False PAC(Seek)=False PAC(CE)=False rangeThroughConvert=False rangeMismatched=False |
| ESC004:bad | rows=0 reads=3 ops=[Index Seek] seekOnText=True convertImplicit=False PAC(Seek)=False PAC(CE)=False rangeThroughConvert=False rangeMismatched=False |
| ESC004:good | rows=1 reads=3 ops=[Index Seek] seekOnText=True convertImplicit=False PAC(Seek)=False PAC(CE)=False rangeThroughConvert=False rangeMismatched=False |
| ESC007:clean | rows=0 reads=3 ops=[Index Seek] seekOnText=True convertImplicit=False PAC(Seek)=False PAC(CE)=False rangeThroughConvert=False rangeMismatched=False |
| ESC005-lower:bad | rows=1 reads=219 ops=[Index Scan] seekOnEmail=False convertImplicit=False PAC(Seek)=False PAC(CE)=False rangeThroughConvert=False rangeMismatched=False |
| ESC005-lower:good | rows=1 reads=3 ops=[Index Seek] seekOnEmail=True convertImplicit=False PAC(Seek)=False PAC(CE)=False rangeThroughConvert=False rangeMismatched=False |
| ESC005-substring:bad | rows=3 reads=192 ops=[Index Scan] seekOnSku=False convertImplicit=False PAC(Seek)=False PAC(CE)=False rangeThroughConvert=False rangeMismatched=False |
| ESC005-substring:good | rows=3 reads=2 ops=[Nested Loops, Compute Scalar, Constant Scan, Index Seek] seekOnSku=True convertImplicit=False PAC(Seek)=False PAC(CE)=False rangeThroughConvert=False rangeMismatched=False |
| ESC005-year:bad | rows=5840 reads=268 ops=[Index Scan] seekOnOrderDate=False convertImplicit=False PAC(Seek)=False PAC(CE)=False rangeThroughConvert=False rangeMismatched=False |
| ESC005-year:good | rows=5840 reads=34 ops=[Index Seek] seekOnOrderDate=True convertImplicit=False PAC(Seek)=False PAC(CE)=False rangeThroughConvert=False rangeMismatched=False |
| ESC005-upper:bad | rows=1 reads=331 ops=[Compute Scalar, Index Scan] seekOnLastName=False convertImplicit=False PAC(Seek)=False PAC(CE)=False rangeThroughConvert=False rangeMismatched=False |
| ESC005-upper:good | rows=1 reads=3 ops=[Compute Scalar, Index Seek] seekOnLastName=True convertImplicit=False PAC(Seek)=False PAC(CE)=False rangeThroughConvert=False rangeMismatched=False |
| ESC005-castdate:clean | rows=16 reads=2 ops=[Nested Loops, Compute Scalar, Constant Scan, Index Seek] seekOnOrderDate=True convertImplicit=False PAC(Seek)=False PAC(CE)=False rangeThroughConvert=True rangeMismatched=True |
| ESC006-suffix:bad | rows=50 reads=331 ops=[Compute Scalar, Index Scan] seekOnLastName=False convertImplicit=False PAC(Seek)=False PAC(CE)=False rangeThroughConvert=False rangeMismatched=False |
| ESC006-contains:bad | rows=50 reads=331 ops=[Compute Scalar, Index Scan] seekOnLastName=False convertImplicit=False PAC(Seek)=False PAC(CE)=False rangeThroughConvert=False rangeMismatched=False |
| ESC006-suffix:good | rows=50 reads=4 ops=[Compute Scalar, Index Seek] seekOnReversedLastName=True convertImplicit=False PAC(Seek)=False PAC(CE)=False rangeThroughConvert=False rangeMismatched=False |
| ESC006-prefix:clean | rows=0 reads=3 ops=[Compute Scalar, Index Seek] seekOnLastName=True convertImplicit=False PAC(Seek)=False PAC(CE)=False rangeThroughConvert=False rangeMismatched=False |
| ESC006-email:bad | rows=1 reads=219 ops=[Nested Loops, Compute Scalar, Constant Scan, Index Seek] seekOnEmail=True convertImplicit=False PAC(Seek)=False PAC(CE)=False rangeThroughConvert=False rangeMismatched=False |
| ESC008-dateadd:bad | rows=1312 reads=268 ops=[Index Scan] seekOnCreatedAt=False convertImplicit=False PAC(Seek)=False PAC(CE)=False rangeThroughConvert=False rangeMismatched=False |
| ESC008-dateadd:good | rows=1312 reads=10 ops=[Index Seek] seekOnCreatedAt=True convertImplicit=False PAC(Seek)=False PAC(CE)=False rangeThroughConvert=False rangeMismatched=False |
| ESC008-maths:bad | rows=19998 reads=268 ops=[Index Scan] seekOnPrice=False convertImplicit=True PAC(Seek)=False PAC(CE)=False rangeThroughConvert=False rangeMismatched=False |
| ESC008-maths:good | rows=19998 reads=109 ops=[Nested Loops, Compute Scalar, Constant Scan, Index Seek] seekOnPrice=True convertImplicit=False PAC(Seek)=False PAC(CE)=False rangeThroughConvert=False rangeMismatched=True |
| ESC009:bad | rows=1 reads=119 ops=[Index Scan] seekOnAccountNumber=False convertImplicit=True PAC(Seek)=True PAC(CE)=True rangeThroughConvert=False rangeMismatched=False |
| ESC009:good | rows=1 reads=2 ops=[Index Seek] seekOnAccountNumber=True convertImplicit=False PAC(Seek)=False PAC(CE)=False rangeThroughConvert=False rangeMismatched=False |
| ESC009-rawint:bad | rows=1 reads=119 ops=[Index Scan] seekOnAccountNumber=False convertImplicit=True PAC(Seek)=True PAC(CE)=True rangeThroughConvert=False rangeMismatched=False |
| ESC010:bad | rows=1 reads=6 ops=[Compute Scalar, Stream Aggregate, Merge Join, Index Seek] seekOnFirstName=True convertImplicit=False PAC(Seek)=False PAC(CE)=False rangeThroughConvert=False rangeMismatched=False |
| ESC010:good | rows=1 reads=6 ops=[Merge Join, Compute Scalar, Index Seek] seekOnFirstName=True convertImplicit=False PAC(Seek)=False PAC(CE)=False rangeThroughConvert=False rangeMismatched=False |
| ESC011:bad | rows=1000 reads=268 ops=[Index Scan] seekOnStatus=False convertImplicit=False PAC(Seek)=False PAC(CE)=False rangeThroughConvert=False rangeMismatched=False |
| ESC011:good | rows=1000 reads=11 ops=[Nested Loops, Merge Interval, Sort, Compute Scalar, Concatenation, Constant Scan, Index Seek] seekOnStatus=True convertImplicit=False PAC(Seek)=False PAC(CE)=False rangeThroughConvert=False rangeMismatched=False |
| ESC012:bad | rows=5825 reads=34 ops=[Index Seek] seekOnOrderDate=True convertImplicit=False PAC(Seek)=False PAC(CE)=False rangeThroughConvert=False rangeMismatched=False |
| ESC012:good | rows=5840 reads=34 ops=[Index Seek] seekOnOrderDate=True convertImplicit=False PAC(Seek)=False PAC(CE)=False rangeThroughConvert=False rangeMismatched=False |
| COLL-sql:bad | rows=1 reads=143 ops=[Index Scan] seekOnEmailSql=False convertImplicit=True PAC(Seek)=True PAC(CE)=False rangeThroughConvert=False rangeMismatched=False |
| COLL-win:bad | rows=1 reads=2 ops=[Nested Loops, Compute Scalar, Constant Scan, Index Seek] seekOnEmailWin=True convertImplicit=True PAC(Seek)=False PAC(CE)=False rangeThroughConvert=True rangeMismatched=False |
