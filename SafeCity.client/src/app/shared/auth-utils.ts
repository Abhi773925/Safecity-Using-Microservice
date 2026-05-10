
export function saveToken(accessToken: string): void {
  localStorage.setItem('accessToken', accessToken);
}

export function getToken(): string {
  return localStorage.getItem('accessToken') || '';
}

export function clearTokens(): void {
  localStorage.removeItem('accessToken');
  localStorage.removeItem('userRole');
  localStorage.removeItem('userEmail');
}

function decodeClaims(token: string): Record<string, any> | null {
  try {
    const payload = token.split('.')[1].replace(/-/g, '+').replace(/_/g, '/');
    return JSON.parse(atob(payload));
  } catch {
    return null;
  }
}

export function getUserRole(): string {
  const claims = decodeClaims(getToken());
  return (
    claims?.['http://schemas.microsoft.com/ws/2008/06/identity/claims/role'] ||
    claims?.['role'] ||
    localStorage.getItem('userRole') ||
    ''
  );
}
export function getUserId(): number | null {
  const claims = decodeClaims(getToken());
  const id = claims?.['UserId'] || claims?.['userId'] || claims?.['sub'];
  const parsed = Number(id);
  return Number.isInteger(parsed) && parsed > 0 ? parsed : null;
}

export function normalizeRole(role: string): string {
  return role.trim().toLowerCase().replace(/\s+/g, '_');
}

export function isTokenValid(): boolean {
  const token = getToken();
  if (!token) return false;
  const claims = decodeClaims(token);
  if (!claims) return false;
  const exp = claims['exp'];
  if (typeof exp !== 'number') return true;
  return exp > Math.floor(Date.now() / 1000);
}
