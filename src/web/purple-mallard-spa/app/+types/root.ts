export namespace Route {
  export interface MetaArgs {
    params: Record<string, string>;
  }

  export interface LinksFunction {
    (): Array<{ rel: string; href: string; crossOrigin?: string }>;
  }

  export interface ErrorBoundaryProps {
    error: any;
  }
}
