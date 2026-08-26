export interface ValidationProblemDetails {
  type?: string;
  title?: string;
  status?: number;
  errors?: Record<string, string[]>;
}

export class ApiError extends Error {
  readonly status: number;
  readonly problem?: ValidationProblemDetails;

  constructor(status: number, problem?: ValidationProblemDetails) {
    super(problem?.title ?? `La API respondió con estado ${status}.`);
    this.name = 'ApiError';
    this.status = status;
    this.problem = problem;
  }

  fieldErrors(): Record<string, string[]> {
    return this.problem?.errors ?? {};
  }
}
