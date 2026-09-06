import { WHATSAPP_NUMBER } from '../../config';
import { WhatsAppIcon } from '../common/icons';

interface WhatsAppButtonProps {
  mensaje: string;
}

// Se oculta por completo si WHATSAPP_NUMBER todavía no está configurado (ver
// config.ts) — mejor no mostrar el botón que mostrar uno que envía a un
// número inventado o vacío.
export function WhatsAppButton({ mensaje }: WhatsAppButtonProps) {
  if (!WHATSAPP_NUMBER) return null;

  const href = `https://wa.me/${WHATSAPP_NUMBER}?text=${encodeURIComponent(mensaje)}`;

  return (
    <a
      href={href}
      target="_blank"
      rel="noopener noreferrer"
      className="flex w-full items-center justify-center gap-2 rounded-lg bg-[#25D366] px-4 py-2.5 text-sm font-semibold text-white hover:bg-[#1ebe57]"
    >
      <WhatsAppIcon className="h-5 w-5" />
      Escríbenos por WhatsApp
    </a>
  );
}
