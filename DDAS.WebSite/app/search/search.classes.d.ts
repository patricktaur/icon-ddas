/// <reference types="core-js" />
export declare class SiteInfo {
    SiteName: string;
    SiteId: number;
    Component: string;
}
export declare class NameSearch {
    NameToSearch: string;
}
export declare class SearchSummary {
    ComplianceFormId: string;
    NameToSearch: string;
    SearchSummaryItems: SearchSummaryItem[];
}
export declare class SearchSummaryItem {
    RecId: string;
    SiteName: string;
    SiteEnum: number;
    SiteUrl: string;
    MatchStatus: string;
}
export declare class StudyNumbers {
    StudyNumber: string;
}
export declare class StudyNumberDetail {
    Name: string;
    Id: number;
}
export declare class SiteResultDetails {
    RecId: string;
    NameToSearch: string;
    SiteEnum: number;
}
export declare class SearchResultSaveData {
    NameToSearch: string;
    SiteEnum: number;
    DataId: string;
    saveSearchDetails: saveSearchDetails[];
}
export declare class saveSearchDetails {
    RowNumber: number;
    Status: string;
}
export declare class SiteData {
    RecId: string;
    CreatedOn: string;
    CreatedBy: number;
    UpdatedOn: string;
    UpdatedBy: string;
    SiteLastUpdatedOn: string;
    Source: string;
    NameToSearch: string;
    SiteEnum: number;
    SiteName: string;
}
export declare class SiteDataItemBase {
    Matched: number;
    RowNumber: number;
    FullName: string;
    Status: string;
    Selected: boolean;
}
export declare class SitesIncludedInSearch {
    SiteName: string;
    SiteEnum: number;
    SiteUrl: number;
    ScannedOn: Date;
    FullMatchCount: number;
    PartialMatchCount: number;
    IssuesIdentified: string;
    Findings: string;
    Issues: string;
    MatchedRecords: MatchedRecordsPerSite[];
}
export declare class MatchedRecordsPerSite {
    Issues: string;
    IssueNumber: number;
    RowNumber: number;
    RecordDetails: string;
    Status: string;
    Selected: boolean;
}
