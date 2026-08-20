@echo off
title MCP Server - Postgres (Port 8002)

set check_compatibility=False
set FASTEMBED_CACHE_PATH=D:\ProgramTools\MyModels
set HF_HOME=D:\ProgramTools\MyModels

:: 오프라인 모드 강제 설정 (인터넷 요청 방지)
set HF_HUB_OFFLINE=1
set TRANSFORMERS_OFFLINE=1

set FASTMCP_SERVER_HOST=0.0.0.0
set FASTMCP_SERVER_PORT=8002
set QDRANT_URL=http://localhost:6333
set COLLECTION_NAME=uptrio-db-sql
set EMBEDDING_MODEL=sentence-transformers/all-MiniLM-L6-v2

echo Starting Postgres MCP Server...
mcp-server-qdrant --transport sse