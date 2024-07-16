import { isRouteErrorResponse, useRouteError } from "react-router-dom";

export function DefaultErrorBoundary() {
  let error = useRouteError() as any;
  return isRouteErrorResponse(error) ? (
    <h1>
      {error.status} {error.statusText}
    </h1>
  ) : (
    <h1>{error.message || error}</h1>
  );
}

// If you want to customize the component display name in React dev tools:
DefaultErrorBoundary.displayName = "DefaultErrorBoundary";
