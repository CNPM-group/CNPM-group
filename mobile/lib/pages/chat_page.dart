import 'package:flutter/material.dart';

import '../api.dart';

class _Message {
  final String text;
  final bool fromUser;
  final bool isError;

  const _Message(this.text, {this.fromUser = false, this.isError = false});
}

class ChatPage extends StatefulWidget {
  const ChatPage({super.key});

  @override
  State<ChatPage> createState() => _ChatPageState();
}

class _ChatPageState extends State<ChatPage> {
  final _input = TextEditingController();
  final _scroll = ScrollController();
  final List<_Message> _messages = [
    const _Message('Chào bạn! Hỏi mình về đơn hàng, trạng thái giao hoặc cách gửi hàng nhé.'),
  ];
  bool _sending = false;

  static const _suggestions = [
    'Đơn gần nhất của tôi tới đâu rồi?',
    'Gửi hàng bằng drone tối đa bao nhiêu kg?',
    'Làm sao để nhận hàng ở trạm?',
  ];

  @override
  void dispose() {
    _input.dispose();
    _scroll.dispose();
    super.dispose();
  }

  Future<void> _send([String? preset]) async {
    final text = (preset ?? _input.text).trim();
    if (text.isEmpty || _sending) return;

    setState(() {
      _messages.add(_Message(text, fromUser: true));
      _sending = true;
      _input.clear();
    });
    _scrollToEnd();

    try {
      final answer = await Api.instance.chat(text);
      if (!mounted) return;
      setState(() => _messages.add(_Message(answer.isEmpty ? '(Không có câu trả lời)' : answer)));
    } on ApiException catch (e) {
      if (!mounted) return;
      setState(() => _messages.add(_Message(e.message, isError: true)));
    } finally {
      if (mounted) setState(() => _sending = false);
      _scrollToEnd();
    }
  }

  void _scrollToEnd() {
    WidgetsBinding.instance.addPostFrameCallback((_) {
      if (_scroll.hasClients) {
        _scroll.animateTo(
          _scroll.position.maxScrollExtent,
          duration: const Duration(milliseconds: 200),
          curve: Curves.easeOut,
        );
      }
    });
  }

  Widget _bubble(_Message m) {
    final scheme = Theme.of(context).colorScheme;
    final color = m.isError
        ? scheme.errorContainer
        : m.fromUser
            ? scheme.primary
            : scheme.secondaryContainer;
    final textColor = m.fromUser ? scheme.onPrimary : scheme.onSecondaryContainer;

    return Align(
      alignment: m.fromUser ? Alignment.centerRight : Alignment.centerLeft,
      child: Container(
        constraints: BoxConstraints(maxWidth: MediaQuery.of(context).size.width * 0.8),
        margin: const EdgeInsets.symmetric(vertical: 4),
        padding: const EdgeInsets.symmetric(horizontal: 14, vertical: 10),
        decoration: BoxDecoration(color: color, borderRadius: BorderRadius.circular(14)),
        child: Text(m.text, style: TextStyle(color: textColor)),
      ),
    );
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(title: const Text('Trợ lý AI')),
      body: Column(
        children: [
          Expanded(
            child: ListView(
              controller: _scroll,
              padding: const EdgeInsets.all(12),
              children: [
                for (final m in _messages) _bubble(m),
                if (_sending)
                  const Padding(
                    padding: EdgeInsets.all(8),
                    child: Text('Đang trả lời...', style: TextStyle(color: Colors.grey)),
                  ),
                if (_messages.length == 1)
                  Wrap(
                    spacing: 8,
                    runSpacing: 8,
                    children: [
                      for (final s in _suggestions) ActionChip(label: Text(s), onPressed: () => _send(s)),
                    ],
                  ),
              ],
            ),
          ),
          SafeArea(
            top: false,
            child: Padding(
              padding: const EdgeInsets.fromLTRB(12, 4, 8, 8),
              child: Row(
                children: [
                  Expanded(
                    child: TextField(
                      controller: _input,
                      minLines: 1,
                      maxLines: 4,
                      textInputAction: TextInputAction.send,
                      onSubmitted: (_) => _send(),
                      decoration: const InputDecoration(
                        hintText: 'Nhập câu hỏi...',
                        border: OutlineInputBorder(),
                        isDense: true,
                      ),
                    ),
                  ),
                  IconButton(
                    onPressed: _sending ? null : () => _send(),
                    icon: const Icon(Icons.send),
                  ),
                ],
              ),
            ),
          ),
        ],
      ),
    );
  }
}
