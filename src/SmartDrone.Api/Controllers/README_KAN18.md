# KAN-18 — LLM API feasibility check

## Included files
- `src/SmartDrone.Api/Controllers/LlmFeasibilityController.cs`: one-call test endpoint `POST /api/llm-feasibility/test`.
- `docs/KAN18_Feasibility_Report.md`: Vietnamese report template. Fill in actual test observations.

## Integration
1. Copy the controller into `src/SmartDrone.Api/Controllers/`.
2. In `src/SmartDrone.Api/Program.cs`, add `builder.Services.AddHttpClient();` before `builder.Build()` if it is not already registered. Do not add it twice.
3. Set `LLM_API_KEY` as an environment variable on the API server. Never commit a real key to GitHub.
4. Optionally merge this section into the existing `appsettings.Development.json`:
   ```json
   {
     "Llm": {
       "ResponsesEndpoint": "https://api.openai.com/v1/responses",
       "Model": "gpt-4.1-mini"
     }
   }
   ```
   Do not overwrite the rest of your settings.
5. The controller has `[Authorize]`, so call it with a valid JWT. Open Swagger and run `POST /api/llm-feasibility/test`.
6. Record the actual HTTP status, latency, token counts, and answer in the report. Do not invent test results.

If no API key is configured, the endpoint returns HTTP 503. The endpoint measures a single request; it does not prove stable performance. Current model availability and pricing must be verified with the provider.
