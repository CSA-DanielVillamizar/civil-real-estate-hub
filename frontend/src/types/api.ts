export interface ValidationProblemDetails {
  type?: string;
  title?: string;
  detail?: string;
  status?: number;
  errors?: Record<string, string[]>;
}

export class ApiError extends Error {
  readonly status: number;
  readonly problem?: ValidationProblemDetails;

  constructor(status: number, problem?: ValidationProblemDetails) {
    // detail trae el mensaje específico de la regla violada (ver
    // ApplicationExceptionHandler.HandleDomainExceptionAsync en el
    // backend — title es siempre el genérico "Se violó una regla de
    // negocio."); se prioriza sobre title cuando está presente.
    super(problem?.detail ?? problem?.title ?? `La API respondió con estado ${status}.`);
    this.name = 'ApiError';
    this.status = status;
    this.problem = problem;
  }

  fieldErrors(): Record<string, string[]> {
    return this.problem?.errors ?? {};
  }
}
