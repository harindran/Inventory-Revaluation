using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EInvoicing_Logitax_API.Business_Objects
{
   public class Querycls
    {
        public int HSNLength=4;
        public string InvoiceQuery(string Docentry)
        {
            string retstring = "";

            retstring = "SELECT Row_number() Over(Partition by b.\"DocEntry\" order by b.\"DocEntry\" Asc )\" SINo\",(B.\"AssVal\"+B.\"Freight Total\") \"AssValN\" ,";
            retstring  = retstring+ " (B.\"AssAmt\"+B.\"CGSTAmt\"+B.\"SGSTAmt\"+B.\"IGSTAmt\") \"Total Item Value\",(B.\"Tot Amt\"-B.\"Tot Amt1\") \"LineDiscountAmt\",B.* FROM(";
            retstring  = retstring+ "  Select B2.*, 'GST' as \"TaxSch\",T.\"BpGSTN\",";
            retstring  = retstring+ " Case when T.\"BpGSTN\"<>'' then Case when T.\"ExportType\"='U' then 'SEZWP' Else 'B2B' End Else '' End as \"SupTyp\" ,'' as \"RegRev\",'INV' Type,a.\"DocEntry\",a.\"DocNum\" \"Inv_No\",a.\"DocType\",a.\"DocDate\" \"Inv_Doc_Date\",ss.\"GSTRegnNo\" \"Seller GSTN\",B1.\"CompnyName\"";
            retstring  = retstring+ " \"Seller_Legal Name\",ss.\"Street\" \"Seller_Addr1\",ss.\"City\" \"Seller Location Name\",Replace(ss.\"ZipCode\",' ','') \"Seller_PIN code\",(select \"GSTCode\" from OCST where \"Country\"=ss.\"Country\" and \"Code\"=ss.\"State\")";
            retstring  = retstring+ " \"Seller_State_code\",a.\"CardCode\",Crd1.\"GSTRegnNo\" \"Buyer GSTN\", a.\"CardName\" \"Buyer_Legal Name\",96 \"Place of supply\", Crd11.\"Address2\" \"BAddress2\",Crd11.\"Address3\" \"BAddress3\",crd11.\"Building\"";
            retstring  = retstring+ " \"BBuilding\",crd11.\"Street\" \"BStreet\",crd11.\"Block\" \"BBlock\",crd11.\"City\" \"BCity\",st1.\"Name\" \"BState\" ,Replace(crd11.\"ZipCode\",' ','') \"BZipCode\",cy1.\"Name\" \"BCountry\", Crd1.\"Address2\" \"SAddress2\",Crd1.\"Address3\" \"SAddress3\",";
            retstring  = retstring+ " crd1.\"Building\" \"SBuilding\",crd1.\"Street\" \"SStreet\",crd1.\"Block\" \"SBlock\",crd1.\"City\" \"SCity\",st.\"Name\" \"SState\" ,Replace(crd1.\"ZipCode\",' ','') \"SZipCode\", cy.\"Name\" \"SCountry\",A.\"GSTTranTyp\",";
            retstring  = retstring+ " (select \"GSTCode\" from OCST where \"Code\"=Crd11.\"State\" and \"Country\"=Crd11.\"Country\") \"Bill to State Code\",(select \"GSTCode\" from OCST where \"Code\"=crd1.\"State\" and \"Country\"=crd1.\"Country\")";
            retstring  = retstring+ " \"Shipp to State Code\",b.\"ItemCode\", b.\"Dscription\",Case when a.\"DocType\"='S' then 'Y' Else 'N' End \"IsServc\",";
            retstring  = retstring+ " Case when a.\"DocType\"='S' then (Select Case when LEft(\"ServCode\",2) like '0%' then Replace(\"ServCode\",'0','') Else \"ServCode\" End from OSAC where b.\"SacEntry\"= \"AbsEntry\") Else Left(Replace(o.\"ChapterID\",'.',''),"+ HSNLength + ") End \"HSN\",Case when a.\"DocType\"='S' then 1 Else b.\"Quantity\" End \"Quantity\",b.\"unitMsr\" \"Unit\",";
            retstring  = retstring+ " Case when a.\"DocType\"='S'then b.\"Price\" else b.\"PriceBefDi\" end \"UnitPrice\",";
            retstring  = retstring+ " Case when a.\"DocType\"='S' then b.\"LineTotal\" Else IFNULL(b.\"PriceBefDi\",0)*b.\"Quantity\" End \"Tot Amt\",";
            retstring  = retstring+ " Case when a.\"DocType\"='S' then b.\"LineTotal\" Else IFNULL(b.\"INMPrice\",0)*b.\"Quantity\" End \"Tot Amt1\",";
            retstring  = retstring+ " Case when a.\"DocType\"='S' then b.\"LineTotal\" Else IFNULL(b.\"INMPrice\",0)*b.\"Quantity\" End \"AssAmt\",(Select Sum(X.\"TaxRate\") From INV4 X Where X.\"DocEntry\"=A.\"DocEntry\" And ";
            retstring  = retstring+ " X.\"LineNum\"=B.\"LineNum\" and X.\"ExpnsCode\"='-1' ) \"GSTRATE\",IFNULL((select sum(\"TaxSum\") from INV4 where \"DocEntry\"=a.\"DocEntry\"and \"staType\"=-100),0) as \"CGSTVal\",";
            retstring  = retstring+ " IFNULL((select sum(\"TaxSum\") from INV4 where \"DocEntry\"=a.\"DocEntry\" and \"staType\"=-110),0) as \"SGSTVal\", IFNULL((select sum(\"TaxSum\") from INV4 where \"DocEntry\"=a.\"DocEntry\" and \"staType\"=-120),0) as \"IGSTVal\",";
            retstring  = retstring+ " IFNULL((select sum(\"TaxSum\") from INV4 where \"DocEntry\"=a.\"DocEntry\" and \"LineNum\"=b.\"LineNum\" and \"staType\"=-100 and \"ExpnsCode\"='-1'),0) as \"CGSTAmt\", IFNULL((select sum(\"TaxSum\") from INV4 where \"DocEntry\"=a.\"DocEntry\" and \"LineNum\"=b.\"LineNum\" and \"staType\"=-110 and \"ExpnsCode\"='-1'),0) as \"SGSTAmt\",";
            retstring  = retstring+ " IFNULL((select sum(\"TaxSum\") from INV4 where \"DocEntry\"=a.\"DocEntry\" and \"LineNum\"=b.\"LineNum\" and \"staType\"=-120 and \"ExpnsCode\"='-1'),0) as \"IGSTAmt\",";
            retstring  = retstring+ " (SELECT MAX(\"BatchNum\") from IBT1 where \"ItemCode\"=b.\"ItemCode\" and \"WhsCode\"=b.\"WhsCode\" and";
            retstring  = retstring+ " \"BaseType\"='13'and \"BaseEntry\"=b.\"DocEntry\" ) AS \"BatchNum\",";
            retstring  = retstring+ " Case when a.\"DocType\"='S' then b.\"LineTotal\" Else (Select Sum(IFNULL(\"INMPrice\",0)*\"Quantity\") from INV1 where \"DocEntry\"=b.\"DocEntry\") End \"AssVal\",IFNULL(a.\"DocTotal\"/a.\"DocRate\",0) \"Doc Total\" ,";
            retstring  = retstring+ " a.\"DocDueDate\" \"Inv Due Date\", a.\"NumAtCard\", a.\"Printed\",a.\"PayToCode\", a.\"ShipToCode\", a.\"Comments\" ,Left(Replace(o.\"ChapterID\",'.','')," + HSNLength + ") \"ChapterID\" , A.\"DiscSum\", A.\"RoundDif\",";
            retstring  = retstring+ " b.\"DiscPrcnt\",((b.\"PriceBefDi\"*\"Quantity\") * (b.\"DiscPrcnt\"/100)) \"LineDisc\",l.\"InvntryUom\" ,IFNULL(a.\"RoundDif\",0) \"Rounding\", a.\"TaxDate\" \"Cust Order Date\", a.\"TotalExpns\",l.\"FrgnName\",a.\"DocCur\",A.\"VatSum\", a.\"TotalExpns\" \"Freight\",";
            retstring  = retstring+ " l.\"SalUnitMsr\",IFNULL(b.\"LineTotal\"/a.\"DocRate\",0) \"Line Total\", IFNULL(a.\"TotalExpns\"/a.\"DocRate\",0) \"Freight Total\",IFNULL(a.\"DiscSum\"/a.\"DocRate\",0) \"Disc Total\",";
            retstring  = retstring+ " (Select \"ServCode\" from OSAC where b.\"SacEntry\"= \"AbsEntry\")\"SacCode\" ,";
            retstring  = retstring+ " i.\"TransID\" ,i.\"TransName\" ,i.\"TransDocNo\" ,i.\"Distance\" ,i.\"TransMode\" ,i.\"VehicleNo\" ,i.\"VehicleTyp\",";
            retstring  = retstring+ " i.\"TransType\",i.\"TransDate\",i.\"SuplyType\",";
            retstring  = retstring+ " i.\"FrmGSTN\" ,i.\"FrmTraName\" ,i.\"FrmAddres1\" ,i.\"FrmAddres2\" ,i.\"FrmPlace\" ,i.\"FrmZipCode\" ,";
            retstring  = retstring+ " i.\"ActFrmStat\" ,i.\"ToGSTN\" ,i.\"ToTraName\" ,i.\"ToAddres1\" ,i.\"ToAddres2\" ,i.\"ToPlace\" ,i.\"ToZipCode\" ,";
            retstring  = retstring+ " i.\"ActToState\",i.\"SubSplyTyp\",ES.\"SubType\" \"SubtypeDescription\", i.\"DocType\" \"EDocType\"";
            retstring  = retstring+ " FROM OINV a INNER JOIN INV1 b on b.\"DocEntry\" = a.\"DocEntry\"";
            retstring  = retstring+ " LEFT JOIN INV26 i ON i.\"DocEntry\" =a.\"DocEntry\"";
            retstring  = retstring+ " left JOIN OCRD g on g.\"CardCode\" = a.\"CardCode\"";
            retstring  = retstring+ " left JOIN OITM l on l.\"ItemCode\" = b.\"ItemCode\"";
            retstring  = retstring+ " left JOIN OCRD m on m.\"CardCode\" = a.\"CardCode\" LEFT JOIN OCPR n on n.\"CardCode\" = a.\"CardCode\" and n.\"CntctCode\" = a.\"CntctCode\"";
            retstring  = retstring+ " LEFT JOIN OCHP o on o.\"AbsEntry\" = l.\"ChapterID\" LEFT JOIN OLCT ss on ss.\"Code\" = b.\"LocCode\"";
            retstring  = retstring+ " LEFT JOIN CRD1 CRD1 on CRD1.\"CardCode\" =a.\"CardCode\" and CRD1.\"Address\" =A.\"ShipToCode\" and CRD1.\"AdresType\" ='S'";
            retstring  = retstring+ " LEFT JOIN OCST st on st.\"Code\"=CRD1.\"State\" and st.\"Country\"=CRD1.\"Country\" LEFT JOIN OCRY cy on cy.\"Code\" =CRD1.\"Country\"";
            retstring  = retstring+ " LEFT JOIN CRD1 crd11 on crd11.\"CardCode\" =a.\"CardCode\" and crd11.\"Address\" =A.\"PayToCode\" and crd11.\"AdresType\"='B'";
            retstring  = retstring+ " LEFT JOIN OCST st1 on st1.\"Code\"=crd11.\"State\" and st1.\"Country\"=crd11.\"Country\" LEFT JOIN OCRY cy1 on cy1.\"Code\" =crd11.\"Country\"";
            retstring  = retstring+ " CROSS JOIN OADM B1";
            retstring  = retstring+ " LEFT JOIN INV12 T ON T.\"DocEntry\"=a.\"DocEntry\"";
            retstring  = retstring+ " LEFT JOIN OEST ES ON ES.\"SubID\" =i.\"SubSplyTyp\"";
            retstring  = retstring+ " LEFT JOIN(SELECT \"BankName\" \"CBankName\",Y.\"BankCode\" \"CBankCode\",\"Branch\" \"CBranch\", \"Account\" \"CAccount\",\"AcctName\" \"CAcctName\",";
            retstring  = retstring+ " X.\"SwiftNum\" \"CIFSCNo\" FROM DSC1 X,ODSC Y Where X.\"AbsEntry\"=Y.\"AbsEntry\" ) B2 On B2.\"CBankCode\"=B1.\"DflBnkCode\"";
            retstring  = retstring+ " WHERE a.\"DocEntry\"="+ Docentry + ")B";

            return retstring;

        }



        public string CreditNoteQuery(string Docentry)
        {
            string retstring = "";

            retstring = "SELECT Row_number () Over (Partition by b.\"DocEntry\" order by b.\"DocEntry\" Asc )\" SINo\",(B.\"AssVal\"+B.\"Freight Total\") \"AssValN\" ,";
            retstring = retstring + " (B.\"AssAmt\"+B.\"CGSTAmt\"+B.\"SGSTAmt\"+B.\"IGSTAmt\") \"Total Item Value\",(B.\"Tot Amt\"-B.\"Tot Amt1\") \"LineDiscountAmt\",";
            retstring = retstring + " B.* FROM(";
            retstring = retstring + " Select B2.*, 'GST' as \"TaxSch\",T.\"BpGSTN\",";
            retstring = retstring + " Case when T.\"BpGSTN\"<>'' then Case when T.\"ExportType\"='U' then 'SEZWP' Else 'B2B' End Else '' End as \"SupTyp\" ,'' as \"RegRev\",'CRN' Type,a.\"DocEntry\",a.\"DocNum\" \"Inv_No\",a.\"DocType\",a.\"DocDate\" \"Inv_Doc_Date\",ss.\"GSTRegnNo\" \"Seller GSTN\",B1.\"CompnyName\"";
            retstring = retstring + " \"Seller_Legal Name\",ss.\"Street\" \"Seller_Addr1\",ss.\"City\" \"Seller Location Name\",Replace(ss.\"ZipCode\",' ','') \"Seller_PIN code\",(select \"GSTCode\" from OCST where \"Country\"=ss.\"Country\" and \"Code\"=ss.\"State\")";
            retstring = retstring + " \"Seller_State_code\",a.\"CardCode\",Crd1.\"GSTRegnNo\" \"Buyer GSTN\", a.\"CardName\" \"Buyer_Legal Name\",96 \"Place of supply\", Crd11.\"Address2\" \"BAddress2\",Crd11.\"Address3\" \"BAddress3\",crd11.\"Building\"";
            retstring = retstring + " \"BBuilding\",crd11.\"Street\" \"BStreet\",crd11.\"Block\" \"BBlock\",crd11.\"City\" \"BCity\",st1.\"Name\" \"BState\" ,Replace(crd11.\"ZipCode\",' ','') \"BZipCode\",cy1.\"Name\" \"BCountry\", Crd1.\"Address2\" \"SAddress2\",Crd1.\"Address3\" \"SAddress3\",";
            retstring = retstring + " crd1.\"Building\" \"SBuilding\",crd1.\"Street\" \"SStreet\",crd1.\"Block\" \"SBlock\",crd1.\"City\" \"SCity\",st.\"Name\" \"SState\" ,Replace(crd1.\"ZipCode\",' ','') \"SZipCode\", cy.\"Name\" \"SCountry\",A.\"GSTTranTyp\",";
            retstring = retstring + " (select \"GSTCode\" from OCST where \"Code\"=Crd11.\"State\" and \"Country\"=Crd11.\"Country\") \"Bill to State Code\",(select \"GSTCode\" from OCST where \"Code\"=crd1.\"State\" and \"Country\"=crd1.\"Country\")";
            retstring = retstring + " \"Shipp to State Code\",b.\"ItemCode\", b.\"Dscription\",Case when a.\"DocType\"='S' then 'Y' Else 'N' End \"IsServc\",";
            retstring = retstring + " Case when a.\"DocType\"='S' then (Select Case when LEft(\"ServCode\",2) like '0%' then Replace(\"ServCode\",'0','') Else \"ServCode\" End from OSAC where b.\"SacEntry\"  = \"AbsEntry\") Else Left(Replace(o.\"ChapterID\",'.','')," + HSNLength + ") End \"HSN\",Case when a.\"DocType\"='S' then 1 Else b.\"Quantity\" End \"Quantity\",b.\"UomCode\" \"Unit\",";
            retstring = retstring + " Case when a.\"DocType\"='S'then b.\"Price\" else b.\"PriceBefDi\" end \"UnitPrice\",";
            retstring = retstring + " Case when a.\"DocType\"='S' then b.\"LineTotal\" Else IFNULL(b.\"PriceBefDi\",0)*b.\"Quantity\" End  \"Tot Amt\",";
            retstring = retstring + " Case when a.\"DocType\"='S' then b.\"LineTotal\" Else IFNULL(b.\"INMPrice\",0)*b.\"Quantity\" End \"Tot Amt1\",";
            retstring = retstring + " Case when a.\"DocType\"='S' then b.\"LineTotal\" Else IFNULL(b.\"INMPrice\",0)*b.\"Quantity\" End \"AssAmt\",(Select Sum(X.\"TaxRate\") From RIN4 X Where X.\"DocEntry\"=A.\"DocEntry\" And";
            retstring = retstring + " X.\"LineNum\"=B.\"LineNum\" and X.\"ExpnsCode\"='-1' ) \"GSTRATE\",IFNULL((select sum(\"TaxSum\") from RIN4 where \"DocEntry\"=a.\"DocEntry\"and \"staType\"=-100),0) as \"CGSTVal\",";
            retstring = retstring + " IFNULL((select sum(\"TaxSum\") from RIN4 where \"DocEntry\"=a.\"DocEntry\" and \"staType\"=-110),0) as \"SGSTVal\", IFNULL((select sum(\"TaxSum\") from RIN4 where \"DocEntry\"=a.\"DocEntry\" and \"staType\"=-120),0) as \"IGSTVal\",";
            retstring = retstring + " IFNULL((select sum(\"TaxSum\") from RIN4 where \"DocEntry\"=a.\"DocEntry\" and \"LineNum\"=b.\"LineNum\" and \"staType\"=-100 and \"ExpnsCode\"='-1'),0) as \"CGSTAmt\", IFNULL((select sum(\"TaxSum\") from RIN4 where \"DocEntry\"=a.\"DocEntry\" and \"LineNum\"=b.\"LineNum\" and \"staType\"=-110 and \"ExpnsCode\"='-1'),0) as \"SGSTAmt\",";
            retstring = retstring + " IFNULL((select sum(\"TaxSum\") from RIN4 where \"DocEntry\"=a.\"DocEntry\" and \"LineNum\"=b.\"LineNum\" and \"staType\"=-120 and \"ExpnsCode\"='-1'),0) as \"IGSTAmt\",";
            retstring = retstring + " (SELECT  MAX(\"BatchNum\") from IBT1 where \"ItemCode\"=b.\"ItemCode\" and \"WhsCode\"=b.\"WhsCode\" and";
            retstring = retstring + " \"BaseType\"='13'and \"BaseEntry\"=b.\"DocEntry\" ) AS   \"BatchNum\",";
            retstring = retstring + " Case when a.\"DocType\"='S' then b.\"LineTotal\" Else (Select Sum(IFNULL(\"INMPrice\",0)*\"Quantity\") from RIN1 where \"DocEntry\"=b.\"DocEntry\") End \"AssVal\",IFNULL(a.\"DocTotal\"/a.\"DocRate\",0) \"Doc Total\" ,";
            retstring = retstring + " a.\"DocDueDate\" \"Inv Due Date\", a.\"NumAtCard\", a.\"Printed\",a.\"PayToCode\", a.\"ShipToCode\", a.\"Comments\" ,Left(Replace(o.\"ChapterID\",'.','')," + HSNLength + ") \"ChapterID\" , A.\"DiscSum\", A.\"RoundDif\",";
            retstring = retstring + " b.\"DiscPrcnt\",((b.\"PriceBefDi\"*\"Quantity\") * (b.\"DiscPrcnt\"/100)) \"LineDisc\",l.\"InvntryUom\" ,IFNULL(a.\"RoundDif\",0) \"Rounding\", a.\"TaxDate\" \"Cust Order Date\", a.\"TotalExpns\",l.\"FrgnName\",a.\"DocCur\",A.\"VatSum\", a.\"TotalExpns\" \"Freight\",";
            retstring = retstring + " l.\"SalUnitMsr\",  IFNULL(b.\"LineTotal\"/a.\"DocRate\",0) \"Line Total\", IFNULL(a.\"TotalExpns\"/a.\"DocRate\",0) \"Freight Total\",IFNULL(a.\"DiscSum\"/a.\"DocRate\",0) \"Disc Total\",";
            retstring = retstring + " (Select \"ServCode\" from OSAC where b.\"SacEntry\"  = \"AbsEntry\")\"SacCode\" ,";
            retstring = retstring + " i.\"TransID\" ,i.\"TransName\" ,i.\"TransDocNo\" ,i.\"Distance\" ,i.\"TransMode\" ,i.\"VehicleNo\" ,i.\"VehicleTyp\",";
            retstring = retstring + " i.\"TransType\",i.\"TransDate\",i.\"SuplyType\",";
            retstring = retstring + " i.\"FrmGSTN\" ,i.\"FrmTraName\" ,i.\"FrmAddres1\" ,i.\"FrmAddres2\" ,i.\"FrmPlace\" ,i.\"FrmZipCode\" ,";
            retstring = retstring + " i.\"ActFrmStat\" ,i.\"ToGSTN\" ,i.\"ToTraName\" ,i.\"ToAddres1\" ,i.\"ToAddres2\" ,i.\"ToPlace\" ,i.\"ToZipCode\" ,";
            retstring = retstring + " i.\"ActToState\",i.\"SubSplyTyp\",ES.\"SubType\" \"SubtypeDescription\" ,i.\"DocType\" \"EDocType\"";
            retstring = retstring + " FROM ORIN a INNER JOIN RIN1 b on b.\"DocEntry\" = a.\"DocEntry\"";
            retstring = retstring + " LEFT  JOIN  RIN26 i  ON i.\"DocEntry\" =a.\"DocEntry\"";
            retstring = retstring + " left JOIN OCRD g on g.\"CardCode\" = a.\"CardCode\"";
            retstring = retstring + " left JOIN OITM l on l.\"ItemCode\" = b.\"ItemCode\"";
            retstring = retstring + " left JOIN OCRD m on m.\"CardCode\" = a.\"CardCode\" LEFT JOIN OCPR n on n.\"CardCode\" = a.\"CardCode\" and n.\"CntctCode\" = a.\"CntctCode\"";
            retstring = retstring + " LEFT JOIN OCHP o on o.\"AbsEntry\" = l.\"ChapterID\" LEFT JOIN OLCT ss on ss.\"Code\" = b.\"LocCode\"";
            retstring = retstring + " LEFT JOIN CRD1 CRD1 on CRD1.\"CardCode\"=a.\"CardCode\" and CRD1.\"Address\" =A.\"ShipToCode\" and CRD1.\"AdresType\" ='S'";
            retstring = retstring + " LEFT JOIN OCST st on st.\"Code\"=CRD1.\"State\" and st.\"Country\"=CRD1.\"Country\" LEFT JOIN OCRY cy on cy.\"Code\" =CRD1.\"Country\"";
            retstring = retstring + " LEFT JOIN CRD1 crd11 on crd11.\"CardCode\" =a.\"CardCode\" and crd11.\"Address\" =A.\"PayToCode\" and crd11.\"AdresType\"='B'";
            retstring = retstring + " LEFT JOIN OCST st1 on st1.\"Code\"=crd11.\"State\" and st1.\"Country\"=crd11.\"Country\" LEFT JOIN OCRY cy1 on cy1.\"Code\" =crd11.\"Country\"";
            retstring = retstring + " CROSS JOIN OADM B1";
            retstring = retstring + " LEFT JOIN RIN12 T ON T.\"DocEntry\"=a.\"DocEntry\"";
            retstring = retstring + " LEFT JOIN OEST ES ON ES.\"SubID\" =i.\"SubSplyTyp\"";
            retstring = retstring + " LEFT JOIN(SELECT \"BankName\" \"CBankName\",Y.\"BankCode\" \"CBankCode\",\"Branch\" \"CBranch\", \"Account\" \"CAccount\",\"AcctName\" \"CAcctName\",";
            retstring = retstring + " X.\"SwiftNum\" \"CIFSCNo\" FROM DSC1 X,ODSC Y Where X.\"AbsEntry\"=Y.\"AbsEntry\" ) B2 On B2.\"CBankCode\"=B1.\"DflBnkCode\"";
            retstring = retstring + " WHERE a.\"DocEntry\"=" + Docentry + ")B";

            return retstring;

        }
    }
}
