import { Rol } from '../core/api/models';

/** Clase de color según el tipo de actividad (preventiva, correctiva, inspección). */
export function claseTipo(nombre?: string | null): string {
  const n = (nombre ?? '').normalize('NFD').replace(/[̀-ͯ]/g, '').toLowerCase();
  if (n.includes('prevent')) return 'preventiva';
  if (n.includes('correct')) return 'correctiva';
  if (n.includes('inspec')) return 'inspeccion';
  return '';
}

export function iniciales(nombre?: string | null): string {
  return (nombre ?? '')
    .trim()
    .split(/\s+/)
    .slice(0, 2)
    .map((p) => p[0]?.toUpperCase() ?? '')
    .join('');
}

export const NOMBRE_ROL: Record<Rol, string> = {
  Root: 'Root',
  Admin: 'Administrador',
  Supervisor: 'Supervisor',
  Tecnico: 'Técnico',
};

/** Fecha local en formato yyyy-MM-dd (los filtros de fecha del backend usan la hora local). */
export function fechaIso(d: Date): string {
  const off = d.getTimezoneOffset() * 60_000;
  return new Date(d.getTime() - off).toISOString().substring(0, 10);
}

/** Lunes de la semana de la fecha dada (00:00 local). */
export function lunesDe(d: Date): Date {
  const r = new Date(d.getFullYear(), d.getMonth(), d.getDate());
  r.setDate(r.getDate() - ((r.getDay() + 6) % 7));
  return r;
}

/** Horas y minutos legibles a partir de horas decimales. */
export function duracion(horas?: number | null): string {
  if (!horas) return '—';
  return horas === 1 ? '1 h' : `${horas} h`;
}
