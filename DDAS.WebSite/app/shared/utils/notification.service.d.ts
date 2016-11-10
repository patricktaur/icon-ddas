export declare class NotificationService {
    private _notifier;
    constructor();
    openConfirmationDialog(message: string, okCallback: () => any): void;
    printSuccessMessage(message: string): void;
    printErrorMessage(message: string): void;
}
