export interface Pagination {
    CurrentPage: number;
    ItemsPerPage: number;
    TotalItems: number;
    TotalPages: number;
}
export declare class PaginatedResult<T> {
    result: T;
    pagination: Pagination;
}
export interface Predicate<T> {
    (item: T): boolean;
}
