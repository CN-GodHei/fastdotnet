export interface SystemBlacklistState {
  tableData: {
    data: APIModel.FdBlacklistDto[];
    total: number;
    loading: boolean;
    param: {
      pageNum: number;
      pageSize: number;
    };
  };
  searchForm: {
    type: string;
    value: string;
    reason: string;
  };
}

export interface BlacklistDialogState {
  dialogVisible: boolean;
  dialogTitle: string;
  submitLoading: boolean;
  formData: APIModel.CreateFdBlacklistDto & APIModel.UpdateFdBlacklistDto & { id?: string };
  formRules: any;
  actionType: string;
  rowData?: APIModel.FdBlacklistDto;
}