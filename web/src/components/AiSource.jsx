// Dòng nhỏ dưới nội dung AI: ai viết ra đoạn này. Không giấu việc server tự ghép khi không có LLM.
export default function AiSource({ source, model, note }) {
  return (
    <p className="ai-source">
      <span className={source === 'llm' ? 'source-tag llm' : 'source-tag rule'}>
        {source === 'llm' ? `AI viết${model ? ` · ${model}` : ''}` : 'Tự ghép từ dữ liệu'}
      </span>
      {note && <span className="muted"> {note}</span>}
    </p>
  );
}
