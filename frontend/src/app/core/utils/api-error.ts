import { HttpErrorResponse } from '@angular/common/http';

/**
 * Extrae el mensaje de error devuelto por la API y, si existen errores de
 * validación de DataAnnotations, los concatena para mostrarlos al usuario.
 */
export function extractApiError(error: unknown, fallback: string): string {
  if (error instanceof HttpErrorResponse) {
    if (error.status === 0) {
      return 'No se pudo conectar con la API. Verifique que el backend esté en ejecución en http://localhost:5080.';
    }

    const body = error.error as
      | { message?: string; errors?: Record<string, string[]> }
      | null;

    if (body?.errors) {
      const details = Object.values(body.errors).flat().filter(Boolean);
      if (details.length > 0) {
        return `${body.message ?? fallback} ${details.join(' ')}`.trim();
      }
    }

    if (body?.message) {
      return body.message;
    }
  }

  return fallback;
}
