interface ApiErrorData {
  message?: string;
  errors?: Record<string, string | string[]>;
}

export interface ApiErrorLike {
  code?: string;
  message?: string;
  response?: {
    data?: ApiErrorData;
  };
}

export const asApiError = (error: unknown): ApiErrorLike => {
  return error as ApiErrorLike;
};

export const getApiMessage = (error: unknown): string | undefined => {
  return asApiError(error).response?.data?.message;
};

export const getValidationMessages = (error: unknown): string[] => {
  const errors = asApiError(error).response?.data?.errors;

  if (!errors) {
    return [];
  }

  return Object.values(errors)
    .flatMap((value) => (Array.isArray(value) ? value : [value]))
    .filter(Boolean);
};
