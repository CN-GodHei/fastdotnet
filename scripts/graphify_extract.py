#!/usr/bin/env python
"""graphify 语义抽取包装器 —— 让 graphify 走任意 OpenAI 兼容网关（opencode / dy / zen / vLLM...）。

背景：graphify 的 `extract --backend openai` 把 base_url 硬编码成 https://api.openai.com/v1，
没有环境变量可以改（只有 ollama 后端可以用 OLLAMA_BASE_URL）。本脚本在调用 CLI 前
改写 llm.BACKENDS["openai"]["base_url"]，于是任何 OpenAI 兼容端点都能当后端用。

用法（PowerShell）：
    $env:OPENAI_API_KEY = "<网关 key>"
    $env:GX_BASE_URL     = "http://183.224.180.166:48317/v1"
    $env:GX_MODEL        = "glm-5.3-flash"
    python scripts/graphify_extract.py . --max-concurrency 2

也可以直接用 scripts/graphify_run.ps1（会自动从 opencode auth.json 取 key）。

所有未识别的参数原样透传给 `graphify extract`，例如：
    python scripts/graphify_extract.py . --token-budget 60000 --max-workers 4
"""
from __future__ import annotations

import os
import runpy
import sys

KNOWN_FLAGS_WITH_VALUE = {
    "--base-url": "GX_BASE_URL",
    "--model": "GX_MODEL",
    "--api-key": "OPENAI_API_KEY",
    "--backend": "GX_BACKEND",
}


def _take_known(argv: list[str]) -> list[str]:
    """抽出包装器自己的参数，剩下的交给 graphify。"""
    out: list[str] = []
    i = 0
    while i < len(argv):
        arg = argv[i]
        name, _, inline = arg.partition("=")
        if name in KNOWN_FLAGS_WITH_VALUE:
            env_name = KNOWN_FLAGS_WITH_VALUE[name]
            if inline:
                os.environ[env_name] = inline
                i += 1
            elif i + 1 < len(argv):
                os.environ[env_name] = argv[i + 1]
                i += 2
            else:
                raise SystemExit(f"{name} 缺少取值")
            continue
        out.append(arg)
        i += 1
    return out


def main() -> None:
    rest = _take_known(sys.argv[1:])

    base_url = (os.environ.get("GX_BASE_URL") or "").strip().rstrip("/")
    model = (os.environ.get("GX_MODEL") or "").strip()
    backend = (os.environ.get("GX_BACKEND") or "openai").strip()
    if not os.environ.get("OPENAI_API_KEY"):
        raise SystemExit("缺少 OPENAI_API_KEY（网关 key）。可用 --api-key 传入。")
    if backend == "openai" and not base_url:
        raise SystemExit("缺少 GX_BASE_URL（OpenAI 兼容端点，形如 https://host/v1）")

    from graphify import llm

    if base_url and backend == "openai":
        llm.BACKENDS["openai"]["base_url"] = base_url
    if model:
        llm.BACKENDS.setdefault(backend, {})["default_model"] = model

    argv = ["graphify", "extract", *rest]
    if model and "--model" not in rest:
        argv += ["--model", model]
    if backend and "--backend" not in rest:
        argv += ["--backend", backend]

    print(f"[wrapper] backend={backend} base_url={base_url or '(default)'} model={model or '(default)'}", flush=True)
    sys.argv = argv
    runpy.run_module("graphify.__main__", run_name="__main__")


if __name__ == "__main__":
    main()
