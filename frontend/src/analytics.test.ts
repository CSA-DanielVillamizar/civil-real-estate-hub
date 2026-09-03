import { afterEach, beforeEach, describe, expect, it, vi } from 'vitest';
import { initAnalytics } from './analytics';

beforeEach(() => {
  document.head.innerHTML = '';
  delete window.gtag;
  delete window.dataLayer;
});

afterEach(() => {
  vi.unstubAllEnvs();
});

describe('initAnalytics', () => {
  it('sin VITE_GA_MEASUREMENT_ID configurado, no inyecta ningún script ni define gtag', () => {
    vi.stubEnv('VITE_GA_MEASUREMENT_ID', '');

    initAnalytics();

    expect(document.head.querySelector('script[src*="googletagmanager"]')).toBeNull();
    expect(window.gtag).toBeUndefined();
  });

  it('con VITE_GA_MEASUREMENT_ID configurado, inyecta el script de gtag.js y lo inicializa', () => {
    vi.stubEnv('VITE_GA_MEASUREMENT_ID', 'G-TEST12345');

    initAnalytics();

    const script = document.head.querySelector<HTMLScriptElement>('script[src*="googletagmanager"]');
    expect(script).not.toBeNull();
    expect(script!.src).toContain('G-TEST12345');
    expect(window.gtag).toBeInstanceOf(Function);
    expect(window.dataLayer).toEqual(
      expect.arrayContaining([
        ['js', expect.any(Date)],
        ['config', 'G-TEST12345', { anonymize_ip: true }],
      ]),
    );
  });
});
